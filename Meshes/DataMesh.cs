using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class DataMesh : IMesh {
  private readonly GL gl;
  public VertexAttributeDescriptor[] Attributes { get; }
  public float[] Vertices { get; }
  public uint[] Indices { get; }
  public Simple2DTexture? DiffuseTexture { get; }
  public Simple2DTexture? SpecularTexture { get; }

  public DataMesh(
    GL gl, VertexAttributeDescriptor[] attributes,
    float[] vertices, uint[] indices,
    Simple2DTexture? diffuseTexture = null,
    Simple2DTexture? specularTexture = null
  ) {
    this.gl = gl ?? throw new ArgumentNullException(nameof(gl));

    SpecularTexture = specularTexture;
    DiffuseTexture = diffuseTexture;
    Attributes = attributes;
    Vertices = vertices;
    Indices = indices;
  }

  public unsafe void Draw() {
    gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() {
    DiffuseTexture?.Dispose();
  }
}