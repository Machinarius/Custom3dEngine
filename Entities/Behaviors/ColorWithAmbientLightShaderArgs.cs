using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities.Behaviors;

public class ColorWithAmbientLightShaderArgs : ITransformationBehavior {
  public ITransformationBehavior.Result Run(double deltaTime, double absoluteTime, SceneObject sceneObject) {
    sceneObject.Shaders.SetUniform("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
    sceneObject.Shaders.SetUniform("lightColor", Vector3.One);
    return ITransformationBehavior.Result.Identity(sceneObject);
  }
}