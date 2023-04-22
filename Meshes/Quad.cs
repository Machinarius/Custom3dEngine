using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class Quad : IMesh {
  public VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 3 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 3, 0, VertexAttributePayloadType.Position),
  };

  // Without any projection or model transformations these are effectively clip-space
  // coordinates in the range [-1, 1]. For example 1 in the X axis is the right-most pixel
  // of the view-port, and -1 the left-most.
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

  public Simple2DTexture? Texture => throw new NotImplementedException();

  private readonly GL gl;

  public Quad(GL gl) {
    this.gl = gl;
  }

  public unsafe void Draw() {
    gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() { }
}