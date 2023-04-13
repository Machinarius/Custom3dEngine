using Machinarius.Custom3dEngine.GLAbstractions;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities.Attributes;

public class SimpleMaterial : IObjectAttribute {
  public void WriteToShader(ShaderProgram shaders, double deltaTime, double absoluteTime) {
    shaders.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
    shaders.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
    shaders.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
    shaders.SetUniform("material.shininess", 32.0f);
  }
}
