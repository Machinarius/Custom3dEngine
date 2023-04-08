using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged {
  private readonly uint handle;
  private readonly BufferTargetARB bufferType;
  private readonly GL gl;

  public BufferObject(GL gl, BufferTargetARB bufferType, ReadOnlySpan<TDataType> data) {
    this.gl = gl;
    this.bufferType = bufferType;

    handle = gl.GenBuffer();
    Bind();
    UploadDataToBuffer(data);
  }

  private unsafe void UploadDataToBuffer(ReadOnlySpan<TDataType> data) {
    fixed (void* rawData = data) {
      gl.BufferData(bufferType, (nuint) (data.Length * sizeof(TDataType)), rawData, BufferUsageARB.StaticDraw);
    }
  }

  public void Bind() {
    gl.BindBuffer(bufferType, handle);
  }

  public void Dispose() {
    gl.DeleteBuffer(handle);
  }
}