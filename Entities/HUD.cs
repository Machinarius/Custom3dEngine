using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class HUD: IDisposable {
  private readonly IWindow window;
  private readonly BufferedMesh quadMesh;
  private readonly ShaderProgram shader;

  // https://www.mbsoftworks.sk/tutorials/opengl4/009-orthographic-2D-projection/
  public HUD(IWindow window, GL gl) {
    if (gl is null) {
      throw new ArgumentNullException(nameof(gl));
    }

    this.window = window ?? throw new ArgumentNullException(nameof(window));

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

  public void Draw() {
    shader.Use();
    shader.SetUniform("uProjection", projectionMatrix, false);
    shader.SetUniform("uModel", modelMatrix);
    
    quadMesh.Bind();
    quadMesh.Draw();
  }

  public void Dispose() {
    window.Resize -= OnWindowResize;
    quadMesh.Dispose();
  }
}