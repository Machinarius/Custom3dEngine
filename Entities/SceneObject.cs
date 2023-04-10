using Machinarius.Custom3dEngine.DebugUtils;
using Machinarius.Custom3dEngine.GLAbstractions;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Entities;

public class SceneObject {
  public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
  public float Scale { get; set; } = 1f;
  public Quaternion Rotation { get; set; } = Quaternion.Identity;

  public BufferedMesh Mesh { get; }
  public ShaderProgram Shaders { get; }

  private readonly string[] RequiredUniforms = new [] {
    "uModel",
    "uView",
    "uProjection"
  };

  private readonly ITransformationBehavior? transformationBehavior;

  public SceneObject(BufferedMesh mesh, ShaderProgram shaders, ITransformationBehavior? transformationBehavior = null) {
    this.transformationBehavior = transformationBehavior;

    Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
    Shaders = shaders ?? throw new ArgumentNullException(nameof(shaders));
    
    var shaderHasAllRequiredUniforms = 
      RequiredUniforms.Aggregate(true, (allPresent, uName) => allPresent && shaders.HasUniform(uName));
    if (!shaderHasAllRequiredUniforms) {
      throw new InvalidOperationException("The shader you wish to attach to this scene object must define the uModel, uView and uProjection uniforms");
    }
  }

  public void Draw(double deltaTime, double absoluteTime, Camera viewSource) {
    Shaders.Use();
    
    Matrix4x4 modelMatrix;
    if (transformationBehavior != null) {
      var result = transformationBehavior.Run(deltaTime, absoluteTime, this);
      modelMatrix = BuildModelMatrix(result.Position, result.Scale, result.Rotation);
    } else {
      modelMatrix = BuildModelMatrix(Position, Scale, Rotation);
    }

    Shaders.SetUniform("uModel", modelMatrix);
    Shaders.SetUniform("uView", viewSource.ViewMatrix);
    Shaders.SetUniform("uProjection", viewSource.ProjectionMatrix);

    Mesh.Draw();
  }

  public void Dispose() {
    Shaders?.Dispose();
    Mesh?.Dispose();
  }

  static private Matrix4x4 BuildModelMatrix(Vector3 position, float scale, Quaternion rotation) {
    return Matrix4x4.Identity * 
      Matrix4x4.CreateFromQuaternion(rotation) * 
      Matrix4x4.CreateScale(scale) * 
      Matrix4x4.CreateTranslation(position);
  }
}
