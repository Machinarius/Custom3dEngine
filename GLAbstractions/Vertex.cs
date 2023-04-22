using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class Vertex {
  public Vector3 Position { get; set; }
  public Vector3? Normal { get; set; }
  public Vector2? UvCoordinates { get; set; }
}