namespace Machinarius.Custom3dEngine.GLAbstractions;

public interface IMesh : IDisposable {
  VertexAttributeDescriptor[] Attributes { get; }
  float[] Vertices { get; }
  uint[]? Indices { get; }
  Simple2DTexture? DiffuseTexture { get; }
  Simple2DTexture? SpecularTexture { get; }
  void Draw();
  WindingOrder WindingOrder => WindingOrder.Clockwise;
}
