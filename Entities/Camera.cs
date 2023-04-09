using Machinarius.Custom3dEngine.Helpers;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class Camera {
  private readonly IWindow window;

  public Vector3 Position { get; set; }
  public Vector3 LookTarget { get; set; }
  public Vector3 Direction => Vector3.Normalize(Position - LookTarget);
  public Vector3 Right => Vector3.Normalize(Vector3.Cross(Vector3.UnitY, Direction));
  public Vector3 Up => Vector3.Cross(Direction, Right);

  public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(
    Position, LookTarget, Up
  );

  public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(
    MathHelper.DegreesToRadians(45), window.Size.X / window.Size.Y, 0.1f, 100
  );

  public Camera(IWindow window) {
    this.window = window ?? throw new ArgumentNullException(nameof(window));

    // Start a few units away from the center of the scene so the contents can be seen
    Position = new Vector3(0, 0, 3);
    LookTarget = Vector3.Zero;
  }
}
