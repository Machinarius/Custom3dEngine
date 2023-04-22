using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class QuadWithColorData : IMesh {
  public VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 7 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 7, 0, VertexAttributePayloadType.Position),
    // 4 floats for RGBA color every 7 elements, starting from the third element
    new VertexAttributeDescriptor(4, VertexAttribPointerType.Float, 7, 3, VertexAttributePayloadType.RgbaColor),
  };
  
  public float[] Vertices => new[] {
    //X     Y      Z     R  G  B  A
      0.5f,  0.5f, 0.0f, 1, 0, 0, 1,
      0.5f, -0.5f, 0.0f, 0, 0, 0, 1,
     -0.5f, -0.5f, 0.0f, 0, 0, 1, 1,
     -0.5f,  0.5f, 0.5f, 0, 0, 0, 1
  };

  public uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };

  public Simple2DTexture? Texture => throw new NotImplementedException();

  private readonly GL gl;

  public QuadWithColorData(GL gl) {
    this.gl = gl;
  }

  public unsafe void Draw() {
    gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() { }
}