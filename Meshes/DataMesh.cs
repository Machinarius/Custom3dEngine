using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class DataMesh : IMesh {
  private readonly GL gl;
  private readonly Simple2DTexture? texture;

  public VertexAttributeDescriptor[] Attributes { get; }
  public float[] Vertices { get; }

  public uint[] Indices => Array.Empty<uint>();

  public DataMesh(GL gl, VertexAttributeDescriptor[] attributes, float[] vertices, Simple2DTexture? texture = null) {
    this.gl = gl ?? throw new ArgumentNullException(nameof(gl));
    this.texture = texture;

    Attributes = attributes;
    Vertices = vertices;
  }

  public void Draw() {
    texture?.Bind(TextureUnit.Texture0);
    gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)Vertices.Length);
  }

  public void Dispose() {
    if (texture != null) {
      texture.Dispose();
    }
  }
}