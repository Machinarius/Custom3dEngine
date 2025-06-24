using Machinarius.Custom3dEngine.GLAbstractions;

namespace Machinarius.Custom3dEngine.Entities.Attributes;

public interface IObjectAttribute {
  public void WriteToShader(ShaderProgram shaders, double deltaTime, double absoluteTime);
}