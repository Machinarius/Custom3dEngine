using Machinarius.Custom3dEngine.DebugUtils;
using Machinarius.Custom3dEngine.Entities;
using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class RenderOrchestrator {
  private readonly IWindow window;
  private readonly ServiceProvider serviceProvider;

  private readonly GL gl;
  private readonly IInputContext inputContext;
  private readonly Camera camera;

  private readonly Scene scene;
  private readonly HeadsUpDisplay headsUpDisplay;

  public RenderOrchestrator(IWindow window) {
    this.window = window;

    gl = window.CreateOpenGL();
    inputContext = window.CreateInput();
    
    // Setup Microsoft dependency injection container
    var services = new ServiceCollection();
    
    // Register core services
    var graphicsContext = new OpenGLContext(gl);
    services.AddSingleton<IGraphicsContext>(graphicsContext);
    services.AddSingleton<IInputContext>(inputContext);
    services.AddSingleton<IWindow>(window);
    services.AddSingleton<GL>(gl);
    services.AddSingleton<IResourceFactory, GraphicsResourceFactory>();
    services.AddSingleton<SceneObjectFactory>();
    services.AddSingleton<SceneBuilder>();
    services.AddSingleton<HeadsUpDisplay>();
    
    serviceProvider = services.BuildServiceProvider();
    
    // Create camera using DI-provided input context
    camera = new Camera(window, serviceProvider.GetRequiredService<IInputContext>(), Vector3.UnitZ * 6, Vector3.UnitY, Vector3.UnitZ * -1);
    
    // Register camera for HeadsUpDisplay
    services.AddSingleton<Camera>(camera);
    serviceProvider = services.BuildServiceProvider();
    
    // Get scene builder and create scene
    var sceneBuilder = serviceProvider.GetRequiredService<SceneBuilder>();
    scene = sceneBuilder.GetScene(camera, window);
    headsUpDisplay = serviceProvider.GetRequiredService<HeadsUpDisplay>();
      
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
    serviceProvider?.Dispose();
    scene?.Dispose();
    headsUpDisplay?.Dispose();
    inputContext?.Dispose();
    gl?.Dispose();
  }
}