using Machinarius.Custom3dEngine.GLAbstractions;

namespace Machinarius.Custom3dEngine.Entities.Attributes;

public class SpecularWithTextureMaterial : IObjectAttribute {
  public void WriteToShader(ShaderProgram shaders, double deltaTime, double absoluteTime) {
    // These two are pointers to the texture units that are bound to the cube's diffuse and specular textures.
    shaders.SetUniform("material.diffuse", 0);
    shaders.SetUniform("material.specular", 1);
    shaders.SetUniform("material.shininess", 32.0f);
  }
}