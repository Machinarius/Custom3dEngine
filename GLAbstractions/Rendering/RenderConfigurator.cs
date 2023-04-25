using Machinarius.Custom3dEngine.DebugUtils;
using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Machinarius.Custom3dEngine.GLAbstractions.Rendering;

public class RenderConfigurator {
  private readonly IWindow window;

  public RenderConfigurator(IWindow window) {
    this.window = window;

    window.Load += OnLoad;
  }

  public void Run() {
    window.Run();
  }

  private GL? gl = null;
  private RenderOrchestrator? renderOrchestrator = null;

  private unsafe void OnLoad() {
    gl = window.CreateOpenGL();

#if DEBUG
    gl.EnableDebugOutput();
#endif

    var vendorNameStr = gl.GetString(GLEnum.Vendor);
    var vendorName = SilkMarshal.PtrToString((nint)vendorNameStr);

    var rendererNameStr = gl.GetString(GLEnum.Renderer);
    var rendererName = SilkMarshal.PtrToString((nint)rendererNameStr);

    var versionStr = gl.GetString(GLEnum.Version);
    var version = SilkMarshal.PtrToString((nint)versionStr);

    Console.WriteLine($"Beginning rendering. Vendor: {vendorName}, Renderer: {rendererName}, OpenGL Version: {version}");

    gl.Enable(GLEnum.DepthTest);

    if (vendorName != null) {
      if (vendorName.Contains("NVIDIA")) {
        //gl.Enable(GLEnum.DepthClamp);
      } else {
        // NVidia drivers don't seem to like this?
        gl.DepthFunc(GLEnum.Greater);
      }

      if (vendorName.Contains("AMD")) {
        // Apparently required on AMD OpenGL implementation
        gl.Enable(GLEnum.CullFace);
      }
    }
    
    gl.FrontFace(GLEnum.Ccw);
    //gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
    gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);

    renderOrchestrator = new RenderOrchestrator(gl, window);
    renderOrchestrator.BeginRendering();
  }
}