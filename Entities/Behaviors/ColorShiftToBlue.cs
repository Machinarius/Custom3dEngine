namespace Machinarius.Custom3dEngine.Entities.Behaviors;

public class ColorShiftToBlue : ITransformationBehavior {
  public ITransformationBehavior.Result Run(double deltaTime, double absoluteTime, SceneObject sceneObject) {
    sceneObject.Shaders.SetUniform("uBlue", (float) Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
    return ITransformationBehavior.Result.Identity(sceneObject);
  }
}
