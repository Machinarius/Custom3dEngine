using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Shaders;

public static class ShaderFactory {
  static private readonly string BasePath = Directory.GetCurrentDirectory();

  public static uint CreateShader(GL gl, ShaderType type, string filename) {
    var shaderPath = Path.Combine(BasePath, "Shaders", filename);
    var shaderText = File.ReadAllText(shaderPath);
    var id = gl.CreateShader(type);
    gl.ShaderSource(id, shaderText);
    gl.CompileShader(id);
    
    var errorLog = gl.GetShaderInfoLog(id);
    if (!string.IsNullOrEmpty(errorLog)) {
      throw new InvalidOperationException($"Could not compile {type} shader '{filename}: {errorLog}'");
    }

    return id;
  }
}