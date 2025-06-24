# OpenGL Error Checking Source Generator

This folder contains a prototype source generator that automatically injects OpenGL error checking into your code. The generator transforms normal GL calls into error-checked versions transparently.

## How It Works

The generator supports **multiple activation modes** - from explicit attributes to completely automatic detection.

## Activation Modes

### 1. **Automatic Detection (No Attributes Required!)**

The generator automatically finds classes with GL usage:

```csharp
// No attributes needed! Generator detects GL field automatically
public class MyRenderer {
    private readonly GL gl;  // Generator detects this
    
    public void Render() {
        gl.ClearColor(Color.Red);        // Gets automatic error checking
        gl.Clear(ClearBufferMask.ColorBufferBit);  // Gets automatic error checking  
        gl.DrawElements(PrimitiveType.Triangles, count, type, null); // Gets automatic error checking
    }
}
```

### 2. **Assembly-Level Configuration**

Configure once for the entire assembly:

```csharp
// In AssemblyInfo.cs or any .cs file
[assembly: GLErrorChecking(DebugOnly = true)]

// Now ALL classes with GL usage automatically get error checking
public class RenderOrchestrator {
    private readonly GL gl;
    // All gl.* calls automatically wrapped - no class attributes needed!
}

public class BufferedMesh {
    private readonly GL gl; 
    // This class also gets automatic error checking!
}
```

### 3. **Project-Level MSBuild Configuration**

Configure via project properties:

```xml
<!-- In .csproj file -->
<PropertyGroup>
    <EnableGLErrorChecking>true</EnableGLErrorChecking>
    <GLErrorCheckingDebugOnly>true</GLErrorCheckingDebugOnly>
</PropertyGroup>
```

```csharp
// No attributes anywhere - just write normal code!
public class MyRenderer {
    private readonly GL gl;
    
    public void Render() {
        gl.ClearColor(Color.Red);  // Automatically gets error checking
    }
}
```

### 4. **Explicit Class-Level Attributes (Original)**

```csharp
[GLErrorChecked(DebugOnly = true)]
public partial class MyRenderer {
    private readonly GL gl;
    
    public void Render() {
        gl.ClearColor(Color.Red);
        gl.Clear(ClearBufferMask.ColorBufferBit);
        gl.DrawElements(PrimitiveType.Triangles, count, type, null);
    }
}
```

### What Gets Generated:
```csharp
public partial class MyRenderer {
    public void Render() {
        gl.ClearColor(Color.Red);
#if DEBUG
        gl.EnsureCallSucceeded();
#endif
        gl.Clear(ClearBufferMask.ColorBufferBit);
#if DEBUG
        gl.EnsureCallSucceeded();
#endif
        gl.DrawElements(PrimitiveType.Triangles, count, type, null);
#if DEBUG
        gl.EnsureCallSucceeded();
#endif
    }
}
```

## Automatic Detection Criteria

The generator automatically processes classes that have:

✅ **GL Field/Property**: `private readonly GL gl;` or `public GL GL { get; }`  
✅ **GL Method Calls**: Any `gl.MethodName()` calls in the class  
✅ **Silk.NET.OpenGL Namespace**: Methods from `Silk.NET.OpenGL.GL` type  

## Automatic Exclusions (Prevents Infinite Recursion)

The generator automatically **excludes** these methods from wrapping:

❌ **Error Checking Methods**: `gl.EnsureCallSucceeded()`, `gl.GetError()`  
❌ **Debug Methods**: Any `gl.Debug*()` methods  
❌ **DebugUtils Extensions**: Any extension methods from DebugUtils namespace  
❌ **User-Specified Exclusions**: Methods listed in `ExcludeMethods` configuration  

This means you can safely mix manual and automatic error checking:

```csharp
public void SafeMixedUsage() {
    gl.DrawElements(...);           // Gets automatic error checking
    gl.EnsureCallSucceeded();       // Manual call - NOT wrapped (prevents recursion)
    gl.BindTexture(...);            // Gets automatic error checking
}
```  

## Configuration Options

```csharp
[GLErrorChecked(
    DebugOnly = true,                    // Only add checks in DEBUG builds
    ExcludeMethods = new[] { "GetError" }, // Skip specific GL methods
    BatchCheck = false                   // Individual vs batched error checking
)]
public partial class MyClass { }

// Or assembly-level:
[assembly: GLErrorChecking(DebugOnly = true)]

// Or project-level in .csproj:
<EnableGLErrorChecking>true</EnableGLErrorChecking>
```

## Priority Order

1. **Class-level attributes** (highest priority)
2. **Assembly-level configuration** 
3. **Project-level MSBuild properties**
4. **Automatic detection** (if any of the above are configured)
```

## Implementation Notes

This is a **prototype implementation**. To use it in production, you would need to:

1. **Create a separate source generator project** with proper CodeAnalysis dependencies
2. **Package it as a NuGet analyzer** for easy distribution
3. **Add comprehensive testing** for different GL call patterns
4. **Handle edge cases** like complex expressions and method chaining

## Benefits

✅ **Zero Runtime Overhead** - All checks are inlined  
✅ **Transparent Syntax** - Write normal GL code  
✅ **Compile-Time Safety** - Automatic error detection  
✅ **Configurable** - Debug-only, method exclusions, etc.  
✅ **IDE Friendly** - Perfect IntelliSense support  

## Files

- `GLErrorCheckedAttribute.cs` - Attributes for marking classes and configuring behavior
- `GLErrorCheckGenerator.cs` - Main source generator implementation
- `README.md` - This documentation

## Future Enhancements

- **Performance monitoring** - Inject timing/profiling code
- **Custom error handlers** - Different error handling strategies
- **Call statistics** - Track GL usage patterns
- **Validation layers** - Additional OpenGL state validation