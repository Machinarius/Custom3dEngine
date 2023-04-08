using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class VertexArrayObject<TVertexType, TIndexType> : IDisposable 
    where TVertexType : unmanaged 
    where TIndexType : unmanaged {
  private readonly uint handle;
  private readonly GL gl;

  public VertexArrayObject(GL gl, BufferObject<TVertexType> vertexBuffer, BufferObject<TIndexType> elementBuffer) {
    this.gl = gl;

    handle = gl.GenVertexArray();

    // Bind these buffers to this VAO
    Bind();
    vertexBuffer.Bind();
    elementBuffer.Bind();
  }

  public void Bind() {
    gl.BindVertexArray(handle);
  }

  /**
    <summary>
    Defines an array of generic vertex attribute data.
    This tells OpenGL how to send the packed data to each individual vertex shader execution.
    Fragment shader arguments _must_ pass through the vertex shader first
    </summary>
    <param name="index">Specifies the index of the generic vertex attribute to be modified.</param>
    <param name="size">Specifies the number of components per generic vertex attribute. Must be 1, 2, 3, 4.</param>
    <param name="type">Specifies the data type of each component in the array</param>
    <param name="stride">Specifies the byte offset between consecutive generic vertex attributes.</param>
    <param name="offset">
      Specifies a offset of the first component of the first generic vertex attribute in the array in the data store of the buffer currently bound to the GL_ARRAY_BUFFER target.
    </param>
  */
  public unsafe void VertexAttribPointer(
    uint index, 
    int size, 
    VertexAttribPointerType type, 
    uint stride, 
    int offset
  ) {
    gl.VertexAttribPointer(index, size, type, false, stride * (uint)sizeof(TVertexType), (void*)(offset * sizeof(TVertexType)));
    gl.EnableVertexAttribArray(index);
  }
  
  public void Dispose() {
    // Only dispose this array because the buffers could be used for other arrays
    gl.DeleteVertexArray(handle);
  }
}
