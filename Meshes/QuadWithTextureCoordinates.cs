using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class QuadWithTextureCoordinates : IMesh {
  private readonly Simple2DTexture texture;

  public VertexAttributeDescriptor[] Attributes => new[] {
    // 3 floats for XYZ coordinates every 5 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 5, 0, VertexAttributePayloadType.Position),
    // 2 floats for UV coordinates every 5 elements, starting from the third element
    new VertexAttributeDescriptor(4, VertexAttribPointerType.Float, 5, 3, VertexAttributePayloadType.TextureCoordinates),
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

  public Simple2DTexture? DiffuseTexture => null;
  public Simple2DTexture? SpecularTexture => null;

  private readonly GL gl;

  public QuadWithTextureCoordinates(GL gl) {
    this.gl = gl;
    texture = new Simple2DTexture(gl, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "silk.png"));
  }

  public unsafe void Draw() {
    texture.Bind(TextureUnit.Texture0);
    gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() {
    texture.Dispose();
  }
}