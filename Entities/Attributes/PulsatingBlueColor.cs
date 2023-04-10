using Machinarius.Custom3dEngine.GLAbstractions;

namespace Machinarius.Custom3dEngine.Entities.Attributes;

public class PulsatingBlueColor : IObjectAttribute {
  public void WriteToShader(ShaderProgram shaders, double deltaTime, double absoluteTime) {
    shaders.SetUniform("uBlue", (float) Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
  }
}
