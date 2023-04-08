using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class BufferedMesh : IDisposable {
  private readonly GL gl;

  public BufferObject<uint> ElementBuffer { get; }
  public BufferObject<float> VertexBuffer { get; }
  public VertexArrayObject<float, uint> VertexArray { get; }

  public IMesh SourceMesh { get; }

  public BufferedMesh(GL gl, IMesh sourceMesh) {
    this.gl = gl;

    SourceMesh = sourceMesh;

    ElementBuffer = new BufferObject<uint>(gl, BufferTargetARB.ElementArrayBuffer, sourceMesh.Indices);
    VertexBuffer = new BufferObject<float>(gl, BufferTargetARB.ArrayBuffer, sourceMesh.Vertices);
    VertexArray = new VertexArrayObject<float, uint>(gl, VertexBuffer, ElementBuffer);
  }

  public void ActivateVertexAttributes() {
    for (uint i = 0; i < SourceMesh.Attributes.Length; i++) {
      var descriptor = SourceMesh.Attributes[i];
      VertexArray.VertexAttribPointer(i, descriptor.ElementCount, descriptor.Type, descriptor.Stride, descriptor.Offset);
    }
  }

  public unsafe void Draw() {
    SourceMesh.PrepareForDrawing();
    gl.DrawElements(PrimitiveType.Triangles, (uint) SourceMesh.Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() {
    ElementBuffer.Dispose();
    VertexBuffer.Dispose();
    VertexArray.Dispose();
  }
}
