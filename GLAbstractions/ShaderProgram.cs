using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class ShaderProgram : IDisposable {
  private readonly uint handle;
  private readonly GL gl;

  public ShaderProgram(GL gl, string vertexFilename, string fragmentFilename) {
    this.gl = gl;

    var vertexId = LoadShader(ShaderType.VertexShader, vertexFilename);
    var fragmentId = LoadShader(ShaderType.FragmentShader, fragmentFilename);

    handle = gl.CreateProgram();
    gl.AttachShader(handle, vertexId);
    gl.AttachShader(handle, fragmentId);
    gl.LinkProgram(handle);

    gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
    if (status == 0) {
      var errorLog = gl.GetProgramInfoLog(handle);
      throw new InvalidOperationException($"Could not create a Program for Vertex shader '{vertexFilename}' and Fragment shader {fragmentFilename}\n{errorLog}");
    }

    // These are no longer valuable by themselves
    gl.DetachShader(handle, vertexId);
    gl.DetachShader(handle, fragmentId);
    gl.DeleteShader(vertexId);
    gl.DeleteShader(fragmentId);
  }

  public void Use() {
    gl.UseProgram(handle);
  }

  public void SetUniform(string name, int value) {
    gl.Uniform1(GetUniformLocation(name), value);
  }

  public void SetUniform(string name, float value) {
    gl.Uniform1(GetUniformLocation(name), value);
  }

  public void Dispose() {
    gl.DeleteProgram(handle); 
  }

  private int GetUniformLocation(string name) {
    var location = gl.GetUniformLocation(handle, name);
    if (location == -1) {
      throw new InvalidOperationException($"Could not locate the '{name}' uniform in this program");
    }
    return location;
  }

  static private readonly string BasePath = Directory.GetCurrentDirectory();

  private uint LoadShader(ShaderType type, string filename) {
    var shaderPath = Path.Combine(BasePath, "Shaders", filename);
    var shaderText = File.ReadAllText(shaderPath);
    var id = gl.CreateShader(type);
    gl.ShaderSource(id, shaderText);
    gl.CompileShader(id);
    
    var errorLog = gl.GetShaderInfoLog(id);
    if (!string.IsNullOrEmpty(errorLog)) {
      throw new InvalidOperationException($"Could not compile {type} shader '{filename}'\n{errorLog}");
    }

    return id;
  }
}