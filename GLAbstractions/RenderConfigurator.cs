using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Machinarius.Custom3dEngine.GLAbstractions; 

public class RenderConfigurator {
  private readonly IWindow window;
  
  public RenderConfigurator(IWindow window) {
    this.window = window;
    window.Load += OnLoad;
  }

  public void Run() {
    window.Run();
  }

  private unsafe void OnLoad() {
    var gl = window.CreateOpenGL();
    Console.WriteLine("OpenGL version " + SilkMarshal.PtrToString((IntPtr)gl.GetString(StringName.Version)));
    Console.WriteLine("Renderer " + SilkMarshal.PtrToString((IntPtr)gl.GetString(StringName.Renderer)));
    Console.WriteLine("Vendor " + SilkMarshal.PtrToString((IntPtr)gl.GetString(StringName.Vendor)));
    Console.WriteLine("GLSL Version " + SilkMarshal.PtrToString((IntPtr)gl.GetString(StringName.ShadingLanguageVersion)));
    
    var orchestrator = new RenderOrchestrator(window);
    orchestrator.ConfigureEventHandlers();
  }
}