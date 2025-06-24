using Machinarius.Custom3dEngine.Helpers;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities.Behaviors;

public class RotationOnXY : ITransformationBehavior {
  public ITransformationBehavior.Result Run(double deltaTime, double absoluteTime, SceneObject sceneObject) {
    var rotationAngle = MathHelper.DegreesToRadians((float)(absoluteTime * 100));
    var modelMatrix = Matrix4x4.CreateRotationX(rotationAngle) * Matrix4x4.CreateRotationY(rotationAngle);
    var quaternion = Quaternion.CreateFromRotationMatrix(modelMatrix);
    return new ITransformationBehavior.Result(sceneObject.Position, sceneObject.Scale, quaternion);
  }
}