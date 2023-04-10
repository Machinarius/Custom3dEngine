using Machinarius.Custom3dEngine.GLAbstractions;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities.Attributes;

public class LitByEmmisive : IObjectAttribute {
  private readonly Vector3 lightPosition;
  private readonly Camera camera;
  private readonly DateTime creationDate;

  public LitByEmmisive(Vector3 lightPosition, Camera camera) {
    creationDate = DateTime.UtcNow;

    this.lightPosition = lightPosition;
    this.camera = camera ?? throw new ArgumentNullException(nameof(camera));
  }

  public void WriteToShader(ShaderProgram shaders, double deltaTime, double absoluteTime) {
    var secondsSinceCreation = (float) (DateTime.UtcNow - creationDate).TotalSeconds;

    var lightColor = Vector3.Zero;
    lightColor.X = MathF.Sin((float) (secondsSinceCreation * 2.0f));
    lightColor.Y = MathF.Sin((float) (secondsSinceCreation * 0.7f));
    lightColor.Z = MathF.Sin((float) (secondsSinceCreation * 1.3f));

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
