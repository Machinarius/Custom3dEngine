namespace Machinarius.Custom3dEngine.Meshes;

public class UploadedMesh {
  public uint VertexArrayObjectId { get; }
  public uint VertexBufferObjectId { get; }
  public uint ElementBufferObjectId { get; }

  private readonly IMesh sourceData;

  public uint[] Indices => sourceData.Indices;

  public UploadedMesh(
    IMesh sourceData,
    uint vertexArrayObjectId, uint vertexBufferObjectId, uint elementBufferObjectId
  ) {
    this.sourceData = sourceData ?? throw new ArgumentNullException(nameof(sourceData));
    VertexArrayObjectId = vertexArrayObjectId;
    VertexBufferObjectId = vertexBufferObjectId;
    ElementBufferObjectId = elementBufferObjectId;
  }
}