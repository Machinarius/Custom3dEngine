using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Shaders;

public static class ProgramFactory {
  public static uint FromShaderFiles(GL gl, string vertexFilename, string fragmentFilename) {
    var vertexId = ShaderFactory.CreateShader(gl, ShaderType.VertexShader, vertexFilename);
    var fragmentId = ShaderFactory.CreateShader(gl, ShaderType.FragmentShader, fragmentFilename);

    var programId = gl.CreateProgram();
    gl.AttachShader(programId, vertexId);
    gl.AttachShader(programId, fragmentId);
    gl.LinkProgram(programId);

    gl.GetProgram(programId, GLEnum.LinkStatus, out var status);
    if (status == 0) {
      var errorLog = gl.GetProgramInfoLog(programId);
      throw new InvalidOperationException($"Could not create a Program for Vertex shader '{vertexFilename}' and Fragment shader {fragmentFilename}\n{errorLog}");
    }

    // These are no longer valuable by themselves
    gl.DetachShader(programId, vertexId);
    gl.DetachShader(programId, fragmentId);
    gl.DeleteShader(vertexId);
    gl.DeleteShader(fragmentId);

    return programId;
  }
}
