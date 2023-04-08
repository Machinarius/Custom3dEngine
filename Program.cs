using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Meshes;
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

    BufferedMesh? quadMesh = null;
    ShaderProgram? shaders = null;
    Simple2DTexture? texture = null;

    using var window = Window.Create(options);
    window.Load += () => {
      glContext = window.CreateOpenGL();
      inputContext = window.CreateInput();

      quadMesh = new BufferedMesh(glContext, new QuadWithTextureCoordinates());
      quadMesh.ActivateVertexAttributes();
      shaders = new ShaderProgram(glContext, "IdentityWithUv.vert", "BasicTextureWithAlphaDiscard.frag");
      texture = new Simple2DTexture(glContext, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "silk.png"));
    };

    window.FramebufferResize += size => {
      glContext?.Viewport(size);
    };

    window.Render += deltaTime => {
      glContext?.ClearColor(System.Drawing.Color.Wheat);
      glContext?.Clear(ClearBufferMask.ColorBufferBit);

      quadMesh?.VertexArray.Bind();
      shaders?.Use();

      texture?.Bind(TextureUnit.Texture0);
      shaders?.SetUniform("uTexture", 0);

      quadMesh?.Draw();

      if (!ShouldGameStillRun(inputContext)) {
        window.Close();
      }
    };

    window.Closing += () => {
      quadMesh?.Dispose();
      shaders?.Dispose();

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
