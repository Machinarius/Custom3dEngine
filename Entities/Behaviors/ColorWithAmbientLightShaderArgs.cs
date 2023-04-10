using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities.Behaviors;

public class ColorWithAmbientLightShaderArgs : ITransformationBehavior {
  private readonly Vector3 lightPosition;
  private readonly Camera camera;

  public ColorWithAmbientLightShaderArgs(Vector3 lightPosition, Camera camera) {
    this.lightPosition = lightPosition;
    this.camera = camera ?? throw new ArgumentNullException(nameof(camera));
  }

  public ITransformationBehavior.Result Run(double deltaTime, double absoluteTime, SceneObject sceneObject) {
    sceneObject.Shaders.SetUniform("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
    sceneObject.Shaders.SetUniform("lightColor", Vector3.One);
    sceneObject.Shaders.SetUniform("lightPos", lightPosition);
    sceneObject.Shaders.SetUniform("cameraPos", camera.Position);
    return ITransformationBehavior.Result.Identity(sceneObject);
  }
}