using Machinarius.Custom3dEngine.Entities.Attributes;
using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Helpers;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.OpenGL;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class SceneObject {
  private readonly IGraphicsContext _graphicsContext;
  public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
  public float Scale { get; set; } = 1f;
  public Quaternion Rotation { get; set; } = Quaternion.Identity;

  public BufferedMesh Mesh { get; }
  public ShaderProgram Shaders { get; }

  private readonly string[] RequiredUniforms = new [] {
    "uModel",
    "uView",
    "uProjection"
  };

  public ITransformationBehavior? TransformationBehavior { get; set; }

  private readonly List<IObjectAttribute> attributes;

  public SceneObject(IGraphicsContext graphicsContext, BufferedMesh mesh, ShaderProgram shaders) {
    _graphicsContext = graphicsContext ?? throw new ArgumentNullException(nameof(graphicsContext));
    attributes = new List<IObjectAttribute>();

    Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
    Shaders = shaders ?? throw new ArgumentNullException(nameof(shaders));
    
    var shaderHasAllRequiredUniforms = 
      RequiredUniforms.Aggregate(true, (allPresent, uName) => allPresent && shaders.HasUniform(uName));
    if (!shaderHasAllRequiredUniforms) {
      throw new InvalidOperationException("The shader you wish to attach to this scene object must define the uModel, uView and uProjection uniforms");
    }
  }

  public void AttachAttribute(IObjectAttribute attribute) {
    attributes.Add(attribute);
  }

  public void Draw(double deltaTime, double absoluteTime, Camera viewSource) {
    var (windingFlag, faceFlag) = Mesh.SourceMesh.WindingOrder switch {
      WindingOrder.Clockwise => (GLEnum.CW, GLEnum.Back),
      WindingOrder.CounterClockwise => (GLEnum.Ccw, GLEnum.Front),
      _ => throw new ArgumentOutOfRangeException(
          "Invalid winding order for mesh: " + Mesh.SourceMesh.WindingOrder
        )
    };
    _graphicsContext.GL.FrontFace(windingFlag);
    _graphicsContext.GL.CullFace(faceFlag);
    
    Mesh.Bind();
    Shaders.Use();

    Mesh.SourceMesh.DiffuseTexture?.Bind(TextureUnit.Texture0);
    Mesh.SourceMesh.SpecularTexture?.Bind(TextureUnit.Texture1);
    
    foreach (var attr in attributes) {
      attr.WriteToShader(Shaders, deltaTime, absoluteTime);
    }
    
    Matrix4x4 modelMatrix;
    if (TransformationBehavior != null) {
      var result = TransformationBehavior.Run(deltaTime, absoluteTime, this);
      modelMatrix = TransformationMath.BuildModelMatrix(result.Position, result.Scale, result.Rotation);
    } else {
      modelMatrix = TransformationMath.BuildModelMatrix(Position, Scale, Rotation);
    }

    Shaders.SetUniform("uModel", modelMatrix);
    Shaders.SetUniform("uView", viewSource.ViewMatrix);
    Shaders.SetUniform("uProjection", viewSource.ProjectionMatrix);

    Shaders.Validate();
    Mesh.Draw();
    
    Mesh.Unbind();
    Mesh.SourceMesh.DiffuseTexture?.Unbind();
  }

  public void Dispose() {
    Shaders?.Dispose();
    Mesh?.Dispose();
  }

}
