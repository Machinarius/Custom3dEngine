namespace Machinarius.Custom3dEngine.Meshes;

public class Quad : IMesh {
  public float[] Vertices => new[] {
      0.5f,  0.5f, 0.0f,
      0.5f, -0.5f, 0.0f,
     -0.5f, -0.5f, 0.0f,
     -0.5f,  0.5f, 0.5f
  };

  public uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };
}