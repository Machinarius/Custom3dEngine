using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class Vertex {
  public const int MaxBoneInfluence = 4;

  public Vector3 Position { get; set; }
  public Vector3? Normal { get; set; }
  public Vector2? UvCoordinates { get; set; }

  public int[]? BoneIds { get; set; }
  public float[]? BoneWeights { get; set; }

  public Vector3? Tangent { get; set; }
  public Vector3? Bitangent { get; set; }
}