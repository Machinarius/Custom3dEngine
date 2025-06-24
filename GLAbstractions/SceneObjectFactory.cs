using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.Entities.Attributes;
using Machinarius.Custom3dEngine.Meshes;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class SceneObjectFactory {
  private readonly IGraphicsContext _graphicsContext;
  private readonly IResourceFactory _resourceFactory;

  public SceneObjectFactory(IGraphicsContext graphicsContext, IResourceFactory resourceFactory) {
    _graphicsContext = graphicsContext ?? throw new ArgumentNullException(nameof(graphicsContext));
    _resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
  }

  public SceneObject CreateLamp(Vector3 position) {
    var mesh = _resourceFactory.CreateMesh(new CubeWithNormalsAndUV(_graphicsContext.GL), "LampMesh");
    mesh.ActivateVertexAttributes();

    var shader = _resourceFactory.CreateShader("IdentityWithMVPAndNormals.vert", "White.frag");

    return new SceneObject(_graphicsContext, mesh, shader) {
      Scale = 0.2f,
      Position = position
    };
  }

  public SceneObject CreateTexturedCube(Vector3 position, Vector3 lightPosition, Camera camera) {
    var model = new Model(_graphicsContext.GL, Path.Combine("Assets", "textured_cube.obj"));
    var shader = _resourceFactory.CreateShader("IdentityWithMVPAndUvAndNormals.vert", "Lighting.frag");

    // Use the first mesh from the model
    var sourceMesh = model.Meshes.First();
    var bufferedMesh = _resourceFactory.CreateMesh(sourceMesh, "ModelMesh");
    bufferedMesh.ActivateVertexAttributes();

    var sceneObject = new SceneObject(_graphicsContext, bufferedMesh, shader) {
      Scale = 0.5f,
      Position = position
    };

    sceneObject.AttachAttribute(new LitByEmmisive(lightPosition, camera));
    sceneObject.AttachAttribute(new SpecularWithTextureMaterial());

    return sceneObject;
  }

  public SceneObject CreateBasicCube(Vector3 position, string vertexShader, string fragmentShader) {
    var mesh = _resourceFactory.CreateMesh(new Cube(_graphicsContext.GL), "BasicCube");
    mesh.ActivateVertexAttributes();

    var shader = _resourceFactory.CreateShader(vertexShader, fragmentShader);

    return new SceneObject(_graphicsContext, mesh, shader) {
      Position = position
    };
  }
}