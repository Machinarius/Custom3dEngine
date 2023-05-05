using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Machinarius.Custom3dEngine;

public class Program {
  public static void Main() {
    var options = WindowOptions.Default;
    options.Title = "My 3d engine";

    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ENABLE_RENDERDOC_WORKAROUND"))) {
      options.TransparentFramebuffer = true;
      options.PreferredBitDepth = new Vector4D<int>(0);
    }

    options.API = new GraphicsAPI {
      API = ContextAPI.OpenGL,
      Profile = ContextProfile.Core,
      Version = new APIVersion(4, 6),
      Flags = ContextFlags.Debug
    };

    using var window = Window.Create(options);
    var renderer = new RenderConfigurator(window);
    renderer.Run();
  }
}
