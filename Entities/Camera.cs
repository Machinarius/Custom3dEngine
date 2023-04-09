using Machinarius.Custom3dEngine.Helpers;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class Camera : IDisposable {
  private const float LookSensitivity = 0.1f;
  private const float MoveSpeed = 2.5f;

  private readonly IWindow window;
  private readonly IMouse? primaryMouse;
  private readonly IKeyboard? primaryKeyboard;

  private Vector2 lastMousePosition;

  public Vector3 Position { get; private set; }
  public Vector3 Direction { get; private set; }
  public Vector3 Up { get; private set; }
  public Vector3 Front { get; private set; }

  public float Yaw { get; private set; }
  public float Pitch { get; private set; }
  public float Zoom { get; private set; }

  public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(
    Position, Position + Front, Up
  );

  public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(
    MathHelper.DegreesToRadians(Zoom), window.Size.X / window.Size.Y, 0.1f, 100
  );

  public Camera(IWindow window, IInputContext input) {
    this.window = window ?? throw new ArgumentNullException(nameof(window));

    // Start a few units away from the center of the scene so the contents can be seen
    Position = new Vector3(0, 0, 3);
    Direction = Vector3.Zero;
    Up = Vector3.UnitY;
    Front = new Vector3(0.0f, 0.0f, -1.0f);

    Yaw = -90f;
    Pitch = 0f;
    Zoom = 45f;

    primaryKeyboard = input.Keyboards[0];
    primaryMouse = input.Mice[0];
    if (primaryMouse != null) {
      primaryMouse.Cursor.CursorMode = CursorMode.Raw;
      primaryMouse.MouseMove += OnMouseMove;
      primaryMouse.Scroll += OnMouseScroll;
    }
  }

  public void Update(double deltaTime) {
    var displacement = (float) deltaTime * MoveSpeed;

    if (primaryKeyboard == null) {
      return;
    }

    if (primaryKeyboard.IsKeyPressed(Key.W)) {
      Position += displacement * Front;
    }

    if (primaryKeyboard.IsKeyPressed(Key.S)) {
      Position -= displacement * Front;
    }

    if (primaryKeyboard.IsKeyPressed(Key.A)) {
      Position -= Vector3.Normalize(Vector3.Cross(Front, Up)) * displacement;
    }

    if (primaryKeyboard.IsKeyPressed(Key.D)) {
      Position += Vector3.Normalize(Vector3.Cross(Front, Up)) * displacement;
    }

    if (primaryKeyboard.IsKeyPressed(Key.Space)) {
      Position += displacement * Up;
    }

    if (primaryKeyboard.IsKeyPressed(Key.ControlLeft)) {
      Position -= displacement * Up;
    }
  }

  private void OnMouseScroll(IMouse mouse, ScrollWheel wheel) {
    Zoom = Math.Clamp(Zoom - wheel.Y, 1.0f, 45f);
  }

  private void OnMouseMove(IMouse mouse, Vector2 mousePosition) {
    if (lastMousePosition == default) {
      lastMousePosition = mousePosition;
      return;
    }

    var xOffset = (mousePosition.X - lastMousePosition.X) * LookSensitivity;
    var yOffset = (mousePosition.Y - lastMousePosition.Y) * LookSensitivity;
    lastMousePosition = mousePosition;

    Yaw += xOffset;
    Pitch -= yOffset;

    Pitch = Math.Clamp(Pitch, -89.0f, 89.0f);
    Direction = new Vector3(
      MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch)),
      MathF.Sin(MathHelper.DegreesToRadians(Pitch)),
      MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch))
    );
    Front = Vector3.Normalize(Direction);
  }

  public void Dispose() {
    if (primaryMouse != null) {
      primaryMouse.MouseMove -= OnMouseMove;
      primaryMouse.Scroll -= OnMouseScroll;
    }
  }
}
