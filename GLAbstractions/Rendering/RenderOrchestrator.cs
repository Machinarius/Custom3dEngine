using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.Entities.Attributes;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions.Rendering;

public class RenderOrchestrator {
  public readonly IWindow window;
  private readonly GL gl;
  private readonly IInputContext inputContext;
  private readonly Camera camera;
  private readonly Scene scene;

  public RenderOrchestrator(GL gl, IWindow window) {
    this.gl = gl ?? throw new ArgumentNullException(nameof(gl));
    this.window = window ?? throw new ArgumentNullException(nameof(window));

    inputContext = window.CreateInput();
    camera = new Entities.Camera(window, inputContext);
    scene = new Scene(camera);
  }

  public void BeginRendering() {
    ConfigureHandlers();
    LoadScene();
  }

  private void ConfigureHandlers() {
    window.FramebufferResize += OnFramebufferResize;
    window.Render += OnRenderRequested;
    window.Update += OnUpdateRequested;
    window.Closing += OnWindowClosing;
  }

  private void LoadScene() {
    var lightPosition = new Vector3(1.2f, 1.0f, 2.0f);
    var model = new Model(gl, Path.Combine("Assets", "textured_cube.obj"));
    var shader = new ShaderProgram(gl, "IdentityWithMVPAndUvAndNormals.vert", "DebugPositionToColor.frag");
    foreach (var mesh in model.Meshes) {
      var bufferedObject = new BufferedMesh(gl, mesh);
      bufferedObject.ActivateVertexAttributes();

      var sceneObject = new SceneObject(bufferedObject, shader);
      //sceneObject.AttachAttribute(new LitByEmmisive(lightPosition, camera));
      scene.Add(sceneObject);
    }

    var lampMesh = new Cube(gl);
    var lampBufferedMesh = new BufferedMesh(gl, lampMesh);
    lampBufferedMesh.ActivateVertexAttributes();

    var lightCubeObject = new SceneObject(lampBufferedMesh, new ShaderProgram(gl, "IdentityWithMVPAndUvAndNormals.vert", "White.frag")) {
      Scale = 0.1f,
      Position = lightPosition
    };
    scene.Add(lightCubeObject);
  }

  private void OnFramebufferResize(Vector2D<int> fbSize) {
    gl.Viewport(fbSize);
  }

  private void OnRenderRequested(double deltaTime) {
    gl.ClearColor(System.Drawing.Color.Gray);
    gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    scene.Draw(deltaTime, window.Time);
  }

  private void OnUpdateRequested(double deltaTime) {
    var keyboard = inputContext.Keyboards.Count > 0 ? inputContext.Keyboards[0] : null;
    if (keyboard != null && keyboard.IsKeyPressed(Key.Escape)) {
      window.Close();
      return;
    }

    camera.Update(deltaTime);
  }

  private void OnWindowClosing() {
    window.FramebufferResize -= OnFramebufferResize;
    window.Render -= OnRenderRequested;
    window.Update -= OnUpdateRequested;
    window.Closing -= OnWindowClosing;

    scene?.Dispose();
    inputContext?.Dispose();
  }
}