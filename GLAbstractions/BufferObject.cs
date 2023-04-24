using Machinarius.Custom3dEngine.DebugUtils;
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
    Console.WriteLine($"Created {bufferType} buffer object with index " + handle);
    Bind();
    UploadDataToBuffer(data);
  }

  private unsafe void UploadDataToBuffer(ReadOnlySpan<TDataType> data) {
    var bufferSize = data.Length * sizeof(TDataType);
    Console.WriteLine($"Uploading {bufferSize} bytes to buffer {handle} of type {bufferType}.");
    fixed (void* rawData = data) {
      gl.BufferData(bufferType, (nuint) bufferSize, rawData, BufferUsageARB.StaticDraw);
      gl.EnsureCallSucceeded();
    }
  }

  public void Bind() {
    gl.BindBuffer(bufferType, handle);
  }

  public void Dispose() {
    gl.DeleteBuffer(handle);
  }
}