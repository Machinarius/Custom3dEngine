using Machinarius.Custom3dEngine.DebugUtils;
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

  private void OnLoad() {
    gl = GL.GetApi(window);

#if DEBUG
    gl.EnableDebugOutput();
#endif

    gl.Enable(GLEnum.DepthTest);
    gl.DepthFunc(GLEnum.Greater);
    
    // Apparently required on AMD OpenGL implementation
    gl.Enable(GLEnum.CullFace);
    gl.FrontFace(GLEnum.Ccw);
    //gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
    // gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);

    renderOrchestrator = new RenderOrchestrator(gl, window);
    renderOrchestrator.BeginRendering();
  }
}