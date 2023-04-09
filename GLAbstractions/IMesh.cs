namespace Machinarius.Custom3dEngine.GLAbstractions;

public interface IMesh : IDisposable {
  Transformation? Transformation { get; set; }
  VertexAttributeDescriptor[] Attributes { get; }
  float[] Vertices { get; }
  uint[] Indices { get; }
  void Draw();
}
