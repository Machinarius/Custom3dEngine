using Machinarius.Custom3dEngine.Helpers;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class Camera : IDisposable {
  private const float LookSensitivity = 0.1f;
  private const float MoveSpeed = 0.75f;

  private readonly IWindow window;
  private readonly IMouse? primaryMouse;
  private readonly IKeyboard? primaryKeyboard;

  private Vector2 lastMousePosition;

  public Vector3 Position { get; private set; }
  public Vector3 Up { get; private set; }
  public Vector3 Front { get; private set; }

  public float Yaw { get; private set; }
  public float Pitch { get; private set; }
  public float Zoom { get; private set; }

  public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(
    Position, Position + Front, Up
  );

  private float ViewportAspectRatio => window.Size.X / (float) window.Size.Y;

  public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(
    MathHelper.DegreesToRadians(Zoom), ViewportAspectRatio, 0.1f, 100f
  );

  public Camera(IWindow window, IInputContext input) : 
    this(window, input, new Vector3(0, 0, 3), Vector3.UnitY, new Vector3(0, 0, -1)) {
    // Start a few units away from the center of the scene so the contents can be seen
  }

  public Camera(IWindow window, IInputContext input, Vector3 position, Vector3 up, Vector3 front) {
    this.window = window ?? throw new ArgumentNullException(nameof(window));

    Position = position;
    Up = up;
    Front = front;

    Yaw = -90f;
    Pitch = 0f;
    Zoom = 45f;

    primaryKeyboard = input.Keyboards[0];
    primaryMouse = input.Mice[0];
    if (primaryMouse != null) {
      window.FocusChanged += OnWindowFocusChanged;

      primaryMouse.MouseMove += OnMouseMove;
      primaryMouse.Scroll += OnMouseScroll;
    }
  }

  private void OnWindowFocusChanged(bool isFocused) {
#if !DEBUG
    var cursorMode = CursorMode.Raw;
    if (!isFocused) {
      cursorMode = CursorMode.Normal;
      lastMousePosition = default;
    }

    if (primaryMouse != null) {
      primaryMouse.Cursor.CursorMode = cursorMode;
    }
#endif
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

    // We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
    Pitch = Math.Clamp(Pitch, -89.0f, 89.0f);

    var direction = Vector3.Zero;
    direction.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
    direction.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
    direction.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

    Front = Vector3.Normalize(direction);
  }

  public void Dispose() {
    if (primaryMouse != null) {
      primaryMouse.MouseMove -= OnMouseMove;
      primaryMouse.Scroll -= OnMouseScroll;
      window.FocusChanged -= OnWindowFocusChanged;
    }
  }
}
