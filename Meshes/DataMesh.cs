using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;
using System.Text.Json;

namespace Machinarius.Custom3dEngine.Meshes;

public class DataMesh : IMesh {
  private readonly GL gl;

  public VertexAttributeDescriptor[] Attributes { get; }
  public float[] Vertices { get; }

  public uint[] Indices { get; }
  public Simple2DTexture? Texture { get; }

  public DataMesh(
    GL gl, VertexAttributeDescriptor[] attributes, 
    float[] vertices, uint[] indices, Simple2DTexture? texture = null
  ) {
    this.gl = gl ?? throw new ArgumentNullException(nameof(gl));
    
    Texture = texture;
    Attributes = attributes;
    Vertices = vertices;
    Indices = indices;
  }

  public unsafe void Draw() {
    //gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)Vertices.Length);
    gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() {
    Texture?.Dispose();
  }
}