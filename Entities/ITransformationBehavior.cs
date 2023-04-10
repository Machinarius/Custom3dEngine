using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public interface ITransformationBehavior {
  public class Result {
    public Vector3 Position { get; } 
    public float Scale { get; } 
    public Quaternion Rotation { get; }

    public Result(Vector3 position, float scale, Quaternion rotation) {
      Position = position;
      Scale = scale;
      Rotation = rotation;
    }

    public static Result Identity(SceneObject sceneObject) {
      return new Result(sceneObject.Position, sceneObject.Scale, sceneObject.Rotation);
    }
  }

  Result Run(
    double deltaTime, double absoluteTime, SceneObject sceneObject
  );
}
