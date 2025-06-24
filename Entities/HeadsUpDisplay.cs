using ImGuiNET;
using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class HeadsUpDisplay: IDisposable {
  private readonly IWindow window;
  private readonly Camera camera;
  private readonly BufferedMesh quadMesh;
  private readonly ShaderProgram shader;
  private readonly ImGuiController imGuiController;
  private readonly IInputContext inputContext;

  // https://www.mbsoftworks.sk/tutorials/opengl4/009-orthographic-2D-projection/
  public HeadsUpDisplay(IWindow window, GL gl, Camera camera, IInputContext inputContext) {
    if (gl is null) {
      throw new ArgumentNullException(nameof(gl));
    }

    this.window = window ?? throw new ArgumentNullException(nameof(window));
    this.camera = camera ?? throw new ArgumentNullException(nameof(camera));
    this.inputContext = inputContext ?? throw new ArgumentNullException(nameof(inputContext));

    ConfigureKeyboardListeners();
    
    imGuiController = new ImGuiController(gl, window, inputContext);
    imGuiController.MakeCurrent();

    window.Resize += OnWindowResize;
    CalculateProjectionMatrix(window.Size);
    CalculateModelMatrix(window.Size);

    shader = new ShaderProgram(gl, "HUDElement.vert", "White.frag");
    quadMesh = new BufferedMesh(gl, new Quad(gl), "HUD quad");
    quadMesh.ActivateVertexAttributes();
  }

  private Matrix4x4 projectionMatrix = Matrix4x4.Identity;
  private Matrix4x4 modelMatrix = Matrix4x4.Identity;

  private void CalculateProjectionMatrix(Vector2D<int> viewportSize) {
    projectionMatrix = Matrix4x4.CreateOrthographic(
      viewportSize.X, viewportSize.Y, -1, 1
    );
  }

  private void CalculateModelMatrix(Vector2D<int> viewportSize) {
    // Normalize the pixel size of the quad to a 200px square
    modelMatrix = Matrix4x4.CreateScale(200f);

    // The translation point is the middle of the screen!
    var left = -((float)viewportSize.X / 2) + 110f;
    var top = -((float)viewportSize.Y / 2) + 110f;
    modelMatrix *= Matrix4x4.CreateTranslation(left, top, 0);
  }

  private void OnWindowResize(Vector2D<int> size) {
    CalculateProjectionMatrix(size);
    CalculateModelMatrix(size);
  }

  public void Update(double deltaTime) {
    imGuiController.Update((float)deltaTime);
  }

  private bool debugVisible;

  public void Draw() {
    shader.Use();
    shader.SetUniform("uProjection", projectionMatrix, false);
    shader.SetUniform("uModel", modelMatrix);
    
    quadMesh.Bind();
    quadMesh.Draw();

    if (debugVisible) {
      RenderDebugHud();
    }
  }

  private void RenderDebugHud() {
    // https://github.com/ImGuiNET/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram/Program.cs
    ImGui.Begin("Debug", 
      ImGuiWindowFlags.AlwaysAutoResize |
      ImGuiWindowFlags.NoDecoration
    );
    ImGui.Text($"Camera position: {camera.Position}");
    
    imGuiController.Render();
  }

  private void ConfigureKeyboardListeners() {
    var mainKeyboard = inputContext.Keyboards.FirstOrDefault();
    if (mainKeyboard == null) {
      return;
    }

    mainKeyboard.KeyDown += OnKeyDown;
  }
  
  private DateTime lastKeyPress = DateTime.Now;

  private void OnKeyDown(IKeyboard keyboard, Key key, int unknown) {
    var timeSinceLastPress = DateTime.Now - lastKeyPress;
    if (timeSinceLastPress < TimeSpan.FromSeconds(0.25)) {
      return;
    }
    lastKeyPress = DateTime.Now;

    if (key != Key.F1) {
      return;
    }

    debugVisible = !debugVisible;
  }

  public void Dispose() {
    window.Resize -= OnWindowResize;
    quadMesh.Dispose();
  }
}