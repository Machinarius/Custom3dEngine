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

  private void OnLoad() {
    var orchestrator = new RenderOrchestrator(window);
    orchestrator.ConfigureEventHandlers();
  }
}