using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public interface IGraphicsContext {
  GL GL { get; }
}