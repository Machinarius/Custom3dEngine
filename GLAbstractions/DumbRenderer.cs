using Machinarius.Custom3dEngine.DebugUtils;
using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class DumbRenderer {
  private readonly IWindow window;

  public DumbRenderer(IWindow window) {
    this.window = window;

    window.Load += OnLoad;
    window.Update += OnUpdate;
    window.Render += OnRender;
    window.Closing += OnClose;
    window.FramebufferResize += OnFramebufferResize;
  }

  public void Run() {
    window.Run();
  }

  private GL? gl = null;
  private IInputContext? inputContext = null;
  private Entities.Camera? camera = null;
  private Model? model = null;
  private ShaderProgram? shader = null;
  private BufferedMesh[]? meshes = null;

  private void OnLoad() {
    gl = GL.GetApi(window);
    inputContext = window.CreateInput();

#if DEBUG
    gl?.EnableDebugOutput();
#endif

    gl?.Enable(GLEnum.DepthTest);
    gl?.DepthFunc(GLEnum.Greater);
    
    // Apparently required on AMD OpenGL implementation
    gl?.Enable(GLEnum.CullFace);
    gl?.FrontFace(GLEnum.Ccw);
    //gl?.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);

    camera = new Entities.Camera(window, inputContext, Vector3.UnitZ * 6, Vector3.UnitY, Vector3.UnitZ * -1);
    model = new Model(gl, Path.Combine("Assets", "textured_cube.obj"));
    shader = new ShaderProgram(gl, "IdentityWithMVPAndUvAndNormals.vert", "BasicTextureWithAlphaDiscard.frag");
    meshes = model.Meshes.Select(mesh => new BufferedMesh(gl, mesh)).ToArray();
    foreach (var mesh in meshes) {
      mesh.ActivateVertexAttributes();
    }

    shader.Validate();

    foreach(var mesh in meshes ?? Array.Empty<BufferedMesh>()) {
      mesh.Bind();
      mesh.SourceMesh.Texture?.Bind();
    }
  }

  private void OnUpdate(double deltaTime) {
    camera?.Update(deltaTime);
    if (inputContext?.Keyboards.FirstOrDefault()?.IsKeyPressed(Key.Escape) ?? false) {
      window.Close();
    }
  }

  private void OnRender(double deltaTime) {
    gl?.ClearColor(System.Drawing.Color.Gray);
    gl?.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    shader?.Use();
    shader?.SetUniform("uModel", Matrix4x4.Identity);
    shader?.SetUniform("uView", camera.ViewMatrix);
    shader?.SetUniform("uProjection", camera.ProjectionMatrix);
    foreach(var mesh in meshes ?? Array.Empty<BufferedMesh>()) {
      mesh.Draw();
    }
  }

  private void OnFramebufferResize(Vector2D<int> size) {
    gl?.Viewport(size);
  }

  private void OnClose() {
    // TODO
  }
}