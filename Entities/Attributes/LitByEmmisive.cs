using Machinarius.Custom3dEngine.GLAbstractions;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities.Attributes;

public class LitByEmmisive : IObjectAttribute {
  private readonly Vector3 lightPosition;
  private readonly Camera camera;

  public LitByEmmisive(Vector3 lightPosition, Camera camera) {
    this.lightPosition = lightPosition;
    this.camera = camera ?? throw new ArgumentNullException(nameof(camera));
  }

  public void WriteToShader(ShaderProgram shaders, double deltaTime, double absoluteTime) {
    var lightColor = Vector3.One;
    var diffuseColor = lightColor * new Vector3(0.5f);
    var ambientColor = diffuseColor * new Vector3(0.2f);

    shaders.SetUniform("cameraPos", camera.Position);
    shaders.SetUniform("light.position", lightPosition);
    shaders.SetUniform("light.ambient", ambientColor);
    shaders.SetUniform("light.diffuse", diffuseColor);
    shaders.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
    shaders.SetUniform("light.position", lightPosition);
  }
}