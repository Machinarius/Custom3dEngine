using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class VertexAttributeDescriptor {
  public int ElementCount { get; }
  public VertexAttribPointerType Type { get; }
  public uint Stride { get; }
  public int Offset { get; }
  public VertexAttributePayloadType PayloadType { get; }

  public VertexAttributeDescriptor(
    int elementCount, VertexAttribPointerType type, uint stride, int offset,
    VertexAttributePayloadType payloadType
  ) {
    ElementCount = elementCount;
    Type = type;
    Stride = stride;
    Offset = offset;
    PayloadType = payloadType;
  }
}

public enum VertexAttributePayloadType {
  Position,
  Normal,
  TextureCoordinates,
  RgbaColor,
}
