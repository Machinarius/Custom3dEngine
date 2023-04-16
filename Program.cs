using Machinarius.Custom3dEngine.DebugUtils;
using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.Entities.Attributes;
using Machinarius.Custom3dEngine.Entities.Behaviors;
using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Helpers;
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

    Camera? camera = null;
    Scene? scene = null;

    using var window = Window.Create(options);
    window.Load += () => {
      glContext = window.CreateOpenGL();
      inputContext = window.CreateInput();

#if DEBUG
      glContext.EnableDebugOutput();
#endif

      glContext.Enable(GLEnum.DepthTest);

      camera = new Camera(window, inputContext, Vector3.UnitZ * 6, Vector3.UnitY, Vector3.UnitZ * -1);
      scene = new Scene(camera);

      var lightPosition = new Vector3(1.2f, 1.0f, 2.0f);
      var model = new Model(glContext, Path.Combine("Assets", "cube.obj"));
      foreach (var mesh in model.Meshes) {
        var bufferedObject = new BufferedMesh(glContext, mesh);
        bufferedObject.ActivateVertexAttributes();

        var sceneObject = new SceneObject(bufferedObject, new ShaderProgram(glContext, "IdentityWithMVPAndUvAndNormals.vert", "Lighting.frag")) {
          //Scale = 0.1f
        };
        sceneObject.AttachAttribute(new LitByEmmisive(lightPosition, camera));
        scene.Add(sceneObject);
      }

      var lampMesh = new CubeWithNormalsAndUV(glContext);
      var lampBufferedMesh = new BufferedMesh(glContext, lampMesh);
      lampBufferedMesh.ActivateVertexAttributes();

      var lightCubeObject = new SceneObject(lampBufferedMesh, new ShaderProgram(glContext, "IdentityWithMVPAndUvAndNormals.vert", "White.frag"));
      scene.Add(lightCubeObject);

      lightCubeObject.Scale = 0.2f;
      lightCubeObject.Position = lightPosition;
    };

    window.FramebufferResize += size => {
      glContext?.Viewport(size);
    };

    window.Update += deltaTime => {
      camera?.Update(deltaTime);
    };

    window.Render += deltaTime => {
      glContext?.ClearColor(System.Drawing.Color.Gray);
      glContext?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      scene?.Draw(deltaTime, window.Time);

      if (!ShouldGameStillRun(inputContext)) {
        window.Close();
      }
    };

    window.Closing += () => {
      camera?.Dispose();
      scene?.Dispose();

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
