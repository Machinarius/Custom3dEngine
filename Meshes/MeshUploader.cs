using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public static class MeshUploader {
  public static UploadedMesh UploadMeshDataToGpu<TMesh>(GL gl, TMesh mesh) where TMesh : IMesh {
    var vaoId = gl.GenVertexArray();
    gl.BindVertexArray(vaoId);

    var vboId = gl.GenBuffer();
    gl.BindBuffer(BufferTargetARB.ArrayBuffer, vboId);
    UploadDataToBuffer(gl, mesh.Vertices, BufferTargetARB.ArrayBuffer);

    var eboId = gl.GenBuffer();
    gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, eboId);
    UploadDataToBuffer(gl, mesh.Indices, BufferTargetARB.ElementArrayBuffer);

    return new UploadedMesh(mesh, vaoId, vboId, eboId);
  }

#pragma warning disable CS8500
  static private unsafe void UploadDataToBuffer<TElements>(GL gl, TElements[] data, BufferTargetARB target) {
    if (data == null) {
      throw new InvalidOperationException("Cannot upload null data to the GPU");
    }

    fixed (void* rawData = &data[0]) {
      gl.BufferData(target, (nuint)(data.Length * sizeof(uint)), rawData, BufferUsageARB.StaticDraw);
    }
  }
#pragma warning restore CS8500
}