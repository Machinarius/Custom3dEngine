namespace Machinarius.Custom3dEngine.GLAbstractions;

public static class ShaderConstants {
  public static uint GetStandardAttributeLocation(VertexAttributePayloadType type) => type switch {
    VertexAttributePayloadType.Position => 0,
    VertexAttributePayloadType.Normal => 1,
    VertexAttributePayloadType.TextureCoordinates => 2,
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
}