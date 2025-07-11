using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class BufferedMesh : IDisposable {
  public BufferObject<uint>? ElementBuffer { get; }
  public BufferObject<float> VertexBuffer { get; }
  public VertexArrayObject<float, uint> VertexArray { get; }

  public IMesh SourceMesh { get; }

  public BufferedMesh(GL gl, IMesh sourceMesh) {
    SourceMesh = sourceMesh;

    if (sourceMesh.Indices is { Length: > 0 }) {
      ElementBuffer = new BufferObject<uint>(gl, BufferTargetARB.ElementArrayBuffer, sourceMesh.Indices);
    }
    VertexBuffer = new BufferObject<float>(gl, BufferTargetARB.ArrayBuffer, sourceMesh.Vertices);
    VertexArray = new VertexArrayObject<float, uint>(gl, VertexBuffer, ElementBuffer);
  }

  public BufferedMesh(GL gl, IMesh sourceMesh, string debugTag) : this(gl, sourceMesh) {
    Console.WriteLine("Created BufferedMesh: " + debugTag);
  }

  public void ActivateVertexAttributes() {
    for (uint i = 0; i < SourceMesh.Attributes.Length; i++) {
      var descriptor = SourceMesh.Attributes[i];
      var pointer = ShaderConstants.GetStandardAttributeLocation(descriptor.PayloadType);
      VertexArray.VertexAttribPointer(pointer, descriptor.ElementCount, descriptor.Type, descriptor.Stride, descriptor.Offset);
    }
  }

  public void Bind() {
    VertexArray.Bind();
  }

  public void Unbind() {
    VertexArray.Unbind();
    VertexBuffer.Unbind();
    ElementBuffer?.Unbind();
  }

  public void Draw() {
    SourceMesh.Draw();
  }

  public void Dispose() {
    ElementBuffer?.Dispose();
    VertexBuffer.Dispose();
    VertexArray.Dispose();
    SourceMesh.Dispose();
  }
}