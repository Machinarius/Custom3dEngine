namespace Machinarius.Custom3dEngine.GLAbstractions;

public interface IMesh {
  VertexAttributeDescriptor[] Attributes { get; }
  float[] Vertices { get; }
  uint[] Indices { get; }
}
