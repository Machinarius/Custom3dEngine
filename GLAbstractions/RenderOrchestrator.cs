using Machinarius.Custom3dEngine.DebugUtils;
using Machinarius.Custom3dEngine.Entities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class RenderOrchestrator {
  private readonly IWindow window;
  private readonly EngineContainer container;

  private readonly GL gl;
  private readonly IInputContext inputContext;
  private readonly Camera camera;

  private readonly Scene scene;
  private readonly HeadsUpDisplay headsUpDisplay;

  public RenderOrchestrator(IWindow window) {
    this.window = window;

    gl = window.CreateOpenGL();
    inputContext = window.CreateInput();
    camera = new Camera(window, inputContext, Vector3.UnitZ * 6, Vector3.UnitY, Vector3.UnitZ * -1);
    
    // Setup dependency injection container
    container = new EngineContainer();
    var graphicsContext = new OpenGLContext(gl);
    var resourceFactory = new GraphicsResourceFactory(graphicsContext);
    var objectFactory = new SceneObjectFactory(graphicsContext, resourceFactory);
    var sceneBuilder = new SceneBuilder(objectFactory);
    
    container.RegisterSingleton<IGraphicsContext>(graphicsContext);
    container.RegisterSingleton<IResourceFactory>(resourceFactory);
    container.RegisterSingleton<SceneObjectFactory>(objectFactory);
    container.RegisterSingleton<SceneBuilder>(sceneBuilder);
    
    scene = sceneBuilder.GetScene(camera, window);
    headsUpDisplay = new HeadsUpDisplay(window, gl, camera);
      
    ConfigureOpenGl();
  }

  public void ConfigureEventHandlers() {
    window.Update += OnUpdate;
    window.Render += OnRender;
    window.Closing += OnClose;
    window.FramebufferResize += OnFramebufferResize;
  }

  private void ConfigureOpenGl() {
#if DEBUG
    gl.EnableDebugOutput();
#endif

    gl.Enable(GLEnum.CullFace);
    gl.Enable(GLEnum.DepthTest);
    //gl?.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);
  }

  private void OnUpdate(double deltaTime) {
    camera.Update(deltaTime);
    headsUpDisplay.Update(deltaTime);
    if (inputContext.Keyboards.FirstOrDefault()?.IsKeyPressed(Key.Escape) ?? false) {
      window.Close();
    }
  }

  private void OnRender(double deltaTime) {
    gl.ClearColor(System.Drawing.Color.Gray);
    gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    scene.Draw(deltaTime, window.Time);
    headsUpDisplay.Draw();
    window.SwapBuffers();
  }

  private void OnFramebufferResize(Vector2D<int> size) {
    gl.Viewport(size);
  }

  private void OnClose() {
    container?.Dispose();
    scene?.Dispose();
    headsUpDisplay?.Dispose();
    inputContext?.Dispose();
    gl?.Dispose();
  }
}