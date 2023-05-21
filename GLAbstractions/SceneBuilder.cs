using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.Entities.Attributes;
using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions; 

public class SceneBuilder {
  public Scene GetScene(GL gl, Camera camera, IWindow window) {
    var inputContext = window.CreateInput();
    var scene = new Scene(camera);

    var lightPosition = new Vector3(1.2f, 1.0f, 2.0f);
    var lampMesh = new CubeWithNormalsAndUV(gl);
    var lampBufferedMesh = new BufferedMesh(gl, lampMesh, "LampMesh");
    lampBufferedMesh.ActivateVertexAttributes();
    var lampShader = new ShaderProgram(gl, "IdentityWithMVPAndNormals.vert", "White.frag", "Lamp");
    scene.Add(new SceneObject(gl, lampBufferedMesh, lampShader) {
      Scale = 0.2f,
      Position = lightPosition
    });
    
    var model = new Model(gl, Path.Combine("Assets", "textured_cube.obj"));
    var sceneShader = new ShaderProgram(gl, "IdentityWithMVPAndUvAndNormals.vert", "Lighting.frag", "Scene");
    var meshes = model.Meshes.Select(mesh => new BufferedMesh(gl, mesh, "ModelMesh")).ToArray();
    foreach (var mesh in meshes) {
      mesh.ActivateVertexAttributes();

      var sceneObject = new SceneObject(gl, mesh, sceneShader) {
        Scale = 0.5f
      };

      sceneObject.AttachAttribute(new LitByEmmisive(lightPosition, camera));
      sceneObject.AttachAttribute(new SpecularWithTextureMaterial());
      scene.Add(sceneObject);
    }

    return scene;
  }
}