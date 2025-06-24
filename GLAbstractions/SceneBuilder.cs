using Machinarius.Custom3dEngine.Entities;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions; 

public class SceneBuilder {
  private readonly SceneObjectFactory _objectFactory;
  
  public SceneBuilder(SceneObjectFactory objectFactory) {
    _objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
  }

  public Scene GetScene(Camera camera, IWindow window) {
    var inputContext = window.CreateInput();
    var scene = new Scene(camera);

    var lightPosition = new Vector3(1.2f, 1.0f, 2.0f);
    
    // Create lamp using factory
    var lamp = _objectFactory.CreateLamp(lightPosition);
    scene.Add(lamp);
    
    // Create textured cube using factory
    var cube = _objectFactory.CreateTexturedCube(Vector3.Zero, lightPosition, camera);
    scene.Add(cube);

    return scene;
  }
}