using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Machinarius.Custom3dEngine;

public class Program {
  public static void Main() {
    GL? glContext = null;
    IInputContext? inputContext = null;

    var options = WindowOptions.Default;
    options.Title = "My 3d engine";

    using var window = Window.Create(options);
    window.Load += () => {
      glContext = window.CreateOpenGL();
      inputContext = window.CreateInput();
    };

    window.FramebufferResize += size => {
      glContext?.Viewport(size);
    };

    window.Render += deltaTime => {
      glContext?.ClearColor(System.Drawing.Color.Wheat);
      glContext?.Clear(ClearBufferMask.ColorBufferBit);
      if (!ShouldGameStillRun(inputContext)) {
        window.Close();
      }
    };

    window.Closing += () => {
      glContext?.Dispose();
      inputContext?.Dispose();
    };

    window.Run();
  }

  static private bool ShouldGameStillRun(IInputContext? inputContext) {
    if (inputContext == null || inputContext.Keyboards.Count < 1) {
      // Can't ESC out when there are no keyboards ¯\_(ツ)_/¯
      return true;
    }

    if (inputContext.Keyboards[0].IsKeyPressed(Key.Escape)) {
      return false;
    }

    return true;
  }
}
