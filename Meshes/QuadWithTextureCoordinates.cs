using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class QuadWithTextureCoordinates : IMesh {
  public VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 5 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 5, 0),
    // 2 floats for UV coordinates every 5 elements, starting from the third element
    new VertexAttributeDescriptor(4, VertexAttribPointerType.Float, 5, 3),
  };
  
  public float[] Vertices => new[] {
    //X     Y      Z     U  V
      0.5f,  0.5f, 0.0f, 1, 0, 
      0.5f, -0.5f, 0.0f, 1, 1, 
     -0.5f, -0.5f, 0.0f, 0, 1, 
     -0.5f,  0.5f, 0.5f, 0, 0
  };

  public uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };
}
