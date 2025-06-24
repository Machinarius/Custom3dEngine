using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Machinarius.Custom3dEngine.DebugUtils;

public static class GLErrorDetector {
  public static void EnsureCallSucceeded(this GL gl, [CallerMemberName] string callerMemberName = "") {
    var error = gl.GetError();
    if (error != GLEnum.NoError) {
      throw new Exception($"{callerMemberName}'s GL call failed: " + error);
    }
  }

  public static unsafe void EnableDebugOutput(this GL gl) {
    gl.Enable(GLEnum.DebugOutput);
    gl.Enable(GLEnum.DebugOutputSynchronous);
    gl.DebugMessageCallback((source, type, id, severity, length, message, param) => {
      var errorString = SilkMarshal.PtrToString(message);
      Console.WriteLine($"Message from OpenGL ({type}) with severity {severity}:");
      Console.WriteLine(errorString);
    }, null);
  }
}