using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinarius.Custom3dEngine.Generators;

[Generator]
public class GLErrorCheckGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
        context.RegisterForSyntaxNotifications(() => new GLSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context) {
        // Check for assembly-level configuration
        var assemblyConfig = GetAssemblyLevelConfig(context.Compilation);
        
        // Check for project-level configuration
        var projectConfig = GetProjectLevelConfig(context);
        
        var defaultConfig = assemblyConfig ?? projectConfig ?? new GLErrorCheckedConfig();
        
        if (context.SyntaxReceiver is not GLSyntaxReceiver receiver) return;

        foreach (var classDeclaration in receiver.AllClasses) {
            var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            var classSymbol = model.GetDeclaredSymbol(classDeclaration);
            
            if (classSymbol == null) continue;

            // Try attribute-based config first, then fall back to defaults
            var config = GetGLErrorCheckedAttribute(classSymbol) ?? 
                        (HasGLUsage(classDeclaration, model) ? defaultConfig : null);
            
            if (config == null) continue;

            var transformedClass = TransformClass(classDeclaration, model, config, context);
            if (transformedClass != null) {
                var sourceText = SourceText.From(transformedClass, Encoding.UTF8);
                context.AddSource($"{classSymbol.Name}_GLErrorChecked.cs", sourceText);
            }
        }
    }

    private GLErrorCheckedConfig? GetAssemblyLevelConfig(Compilation compilation) {
        var assemblyAttributes = compilation.Assembly.GetAttributes();
        var glAttribute = assemblyAttributes.FirstOrDefault(a => 
            a.AttributeClass?.Name == "GLErrorCheckingAttribute");
            
        if (glAttribute == null) return null;
        
        var config = new GLErrorCheckedConfig();
        // Parse assembly-level configuration...
        return config;
    }
    
    private GLErrorCheckedConfig? GetProjectLevelConfig(GeneratorExecutionContext context) {
        // Read MSBuild properties
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
            "build_property.EnableGLErrorChecking", out var enabled);
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
            "build_property.GLErrorCheckingDebugOnly", out var debugOnly);
            
        if (enabled != "true") return null;
        
        return new GLErrorCheckedConfig {
            DebugOnly = debugOnly == "true"
        };
    }
    
    private bool HasGLUsage(ClassDeclarationSyntax classDeclaration, SemanticModel model) {
        // Check for GL field/property
        var hasGLField = classDeclaration.Members
            .OfType<FieldDeclarationSyntax>()
            .Any(f => f.Declaration.Type.ToString().Contains("GL"));
            
        var hasGLProperty = classDeclaration.Members
            .OfType<PropertyDeclarationSyntax>()
            .Any(p => p.Type.ToString().Contains("GL"));
            
        // Check for GL method calls
        var hasGLCalls = classDeclaration.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Any(inv => IsGLMethodCall(inv, model));
            
        return hasGLField || hasGLProperty || hasGLCalls;
    }

    private GLErrorCheckedConfig? GetGLErrorCheckedAttribute(INamedTypeSymbol classSymbol) {
        var attribute = classSymbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == "GLErrorCheckedAttribute");
            
        if (attribute == null) return null;

        var config = new GLErrorCheckedConfig();
        
        foreach (var namedArg in attribute.NamedArguments) {
            switch (namedArg.Key) {
                case "DebugOnly":
                    config.DebugOnly = namedArg.Value.Value as bool? ?? true;
                    break;
                case "BatchCheck":
                    config.BatchCheck = namedArg.Value.Value as bool? ?? false;
                    break;
                case "ExcludeMethods":
                    if (namedArg.Value.Values != null) {
                        config.ExcludeMethods = namedArg.Value.Values
                            .Select(v => v.Value?.ToString())
                            .Where(s => s != null)
                            .ToArray()!;
                    }
                    break;
            }
        }
        
        return config;
    }

    private string? TransformClass(ClassDeclarationSyntax classDeclaration, 
        SemanticModel model, GLErrorCheckedConfig config, GeneratorExecutionContext context) {
        
        var namespaceName = GetNamespaceName(classDeclaration);
        var className = classDeclaration.Identifier.ValueText;
        
        var rewriter = new GLCallRewriter(model, config);
        var transformedClass = (ClassDeclarationSyntax)rewriter.Visit(classDeclaration);
        
        if (!rewriter.HasTransformations) {
            return null; // No GL calls found, skip generation
        }

        var compilationUnit = SyntaxFactory.CompilationUnit()
            .WithUsings(SyntaxFactory.List(new[] {
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Machinarius.Custom3dEngine.DebugUtils")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Silk.NET.OpenGL")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.CompilerServices"))
            }));

        if (!string.IsNullOrEmpty(namespaceName)) {
            var namespaceDecl = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
                .WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(transformedClass));
            compilationUnit = compilationUnit.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceDecl));
        } else {
            compilationUnit = compilationUnit.WithMembers(SyntaxFactory.SingletonList<MemberDeclarationSyntax>(transformedClass));
        }

        return compilationUnit.NormalizeWhitespace().ToFullString();
    }

    private string GetNamespaceName(ClassDeclarationSyntax classDeclaration) {
        var namespaceDecl = classDeclaration.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
        return namespaceDecl?.Name.ToString() ?? "";
    }
}

internal class GLSyntaxReceiver : ISyntaxReceiver {
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();
    public List<ClassDeclarationSyntax> AllClasses { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
        if (syntaxNode is ClassDeclarationSyntax classDeclaration) {
            AllClasses.Add(classDeclaration);
            
            // Keep the old behavior for attributed classes
            if (classDeclaration.AttributeLists.Count > 0) {
                CandidateClasses.Add(classDeclaration);
            }
        }
    }
}

internal class GLErrorCheckedConfig {
    public bool DebugOnly { get; set; } = true;
    public bool BatchCheck { get; set; } = false;
    public string[] ExcludeMethods { get; set; } = new string[0];
}

internal class GLCallRewriter : CSharpSyntaxRewriter {
    private readonly SemanticModel _model;
    private readonly GLErrorCheckedConfig _config;
    
    public bool HasTransformations { get; private set; }

    public GLCallRewriter(SemanticModel model, GLErrorCheckedConfig config) {
        _model = model;
        _config = config;
    }

    public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
        // Check if method should be skipped
        if (HasSkipAttribute(node)) {
            return base.VisitMethodDeclaration(node);
        }

        return base.VisitMethodDeclaration(node);
    }

    public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node) {
        if (node.Expression is InvocationExpressionSyntax invocation && IsGLMethodCall(invocation)) {
            HasTransformations = true;
            return CreateErrorCheckedStatement(node, invocation);
        }

        return base.VisitExpressionStatement(node);
    }

    public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node) {
        // Handle: var result = gl.GetSomething();
        if (node.Declaration.Variables.Count == 1) {
            var variable = node.Declaration.Variables[0];
            if (variable.Initializer?.Value is InvocationExpressionSyntax invocation && IsGLMethodCall(invocation)) {
                HasTransformations = true;
                return CreateErrorCheckedDeclaration(node, invocation);
            }
        }

        return base.VisitLocalDeclarationStatement(node);
    }

    public override SyntaxNode VisitAssignmentExpression(AssignmentExpressionSyntax node) {
        // Handle: result = gl.GetSomething();
        if (node.Right is InvocationExpressionSyntax invocation && IsGLMethodCall(invocation)) {
            HasTransformations = true;
            // Assignment expressions need to be handled in their parent statement
        }

        return base.VisitAssignmentExpression(node);
    }

    private bool IsGLMethodCall(InvocationExpressionSyntax invocation) {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) {
            return false;
        }

        var symbolInfo = _model.GetSymbolInfo(invocation);
        if (symbolInfo.Symbol is not IMethodSymbol method) {
            return false;
        }

        // Check if it's a GL method call
        var isGLCall = method.ContainingType.Name == "GL" && 
                      method.ContainingType.ContainingNamespace.ToDisplayString() == "Silk.NET.OpenGL";

        if (!isGLCall) return false;

        // EXCLUDE error checking methods to prevent recursion
        if (method.Name == "EnsureCallSucceeded" || 
            method.Name == "GetError" ||
            method.Name.StartsWith("Debug")) {
            return false;
        }

        // EXCLUDE extension methods from DebugUtils namespace
        if (method.IsExtensionMethod && 
            method.ContainingType.ContainingNamespace.ToDisplayString().Contains("DebugUtils")) {
            return false;
        }

        // Check if method is in user exclusion list
        if (_config.ExcludeMethods.Contains(method.Name)) {
            return false;
        }

        return true;
    }

    private StatementSyntax CreateErrorCheckedStatement(ExpressionStatementSyntax originalStatement, 
        InvocationExpressionSyntax invocation) {
        
        var errorCheckCall = CreateErrorCheckCall();
        
        if (_config.DebugOnly) {
            // Wrap error check in #if DEBUG
            var block = SyntaxFactory.Block(
                originalStatement,
                SyntaxFactory.IfDirectiveTrivia(
                    SyntaxFactory.IdentifierName("DEBUG"), true, false, false),
                SyntaxFactory.ExpressionStatement(errorCheckCall),
                SyntaxFactory.EndIfDirectiveTrivia(false)
            );
            
            return block;
        } else {
            // Always include error check
            return SyntaxFactory.Block(
                originalStatement,
                SyntaxFactory.ExpressionStatement(errorCheckCall)
            );
        }
    }

    private StatementSyntax CreateErrorCheckedDeclaration(LocalDeclarationStatementSyntax originalDeclaration,
        InvocationExpressionSyntax invocation) {
        
        var errorCheckCall = CreateErrorCheckCall();
        
        if (_config.DebugOnly) {
            return SyntaxFactory.Block(
                originalDeclaration,
                SyntaxFactory.IfDirectiveTrivia(
                    SyntaxFactory.IdentifierName("DEBUG"), true, false, false),
                SyntaxFactory.ExpressionStatement(errorCheckCall),
                SyntaxFactory.EndIfDirectiveTrivia(false)
            );
        } else {
            return SyntaxFactory.Block(
                originalDeclaration,
                SyntaxFactory.ExpressionStatement(errorCheckCall)
            );
        }
    }

    private InvocationExpressionSyntax CreateErrorCheckCall() {
        // Generate: gl.EnsureCallSucceeded();
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("gl"),
                SyntaxFactory.IdentifierName("EnsureCallSucceeded")))
            .WithArgumentList(SyntaxFactory.ArgumentList());
    }

    private bool HasSkipAttribute(MethodDeclarationSyntax method) {
        return method.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString().Contains("SkipGLErrorCheck"));
    }
}