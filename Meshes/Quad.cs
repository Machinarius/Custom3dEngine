using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class Quad : IMesh {
  public VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 3 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 3, 0),
  };

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