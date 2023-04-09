using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine;

public class Program {
  public static void Main() {
    GL? glContext = null;
    IInputContext? inputContext = null;

    var options = WindowOptions.Default;
    options.Title = "My 3d engine";

    IMesh? meshData = null;
    BufferedMesh? bufferedMesh = null;

    using var window = Window.Create(options);
    var camera = new Camera(window);

    window.Load += () => {
      glContext = window.CreateOpenGL();
      inputContext = window.CreateInput();

      glContext.Enable(GLEnum.DepthTest);

      meshData =  new CubeMesh(glContext, camera);
      bufferedMesh = new BufferedMesh(glContext, meshData);
      bufferedMesh.ActivateVertexAttributes();
      
      meshData.Transformation = new Transformation {
        Rotation = Quaternion.CreateFromYawPitchRoll(2f, 1f, 3f)
      };
    };

    window.FramebufferResize += size => {
      glContext?.Viewport(size);
    };

    window.Render += deltaTime => {
      glContext?.ClearColor(System.Drawing.Color.Wheat);
      glContext?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      bufferedMesh?.VertexArray.Bind();
      bufferedMesh?.Draw(deltaTime, window.Time);

      if (!ShouldGameStillRun(inputContext)) {
        window.Close();
      }
    };

    window.Closing += () => {
      bufferedMesh?.Dispose();
      meshData?.Dispose();

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
