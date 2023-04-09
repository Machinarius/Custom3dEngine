using Machinarius.Custom3dEngine.Entities;
using Machinarius.Custom3dEngine.GLAbstractions;
using Machinarius.Custom3dEngine.Helpers;
using Silk.NET.OpenGL;
using System.Numerics;

namespace Machinarius.Custom3dEngine.Meshes;

public class CubeWithTextureData : BaseMesh {
  private readonly Simple2DTexture texture;

  private const string vertexShaderName = "IdentityWithMVPAndUv.vert";

  private const string fragmentShaderName = "BasicTextureWithAlphaDiscard.frag";

  public override VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 5 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 5, 0),
    // 2 floats for UV coordinates every 5 elements, starting from the third element
    new VertexAttributeDescriptor(2, VertexAttribPointerType.Float, 5, 3),
  };

  public override float[] Vertices => new [] {
    //X    Y      Z     U   V
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
      0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
      0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
      0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
      0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
      0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
      0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

      0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
      0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
      0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
      0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
      0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
      0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
      0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
      0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
      0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
      0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
      0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
      0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
  };

  public override uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3,
  };

  public override Transformation? Transformation { get; set; }

  protected override ShaderProgram Shaders { get; }

  private readonly GL gl;
  private readonly Camera camera;

  public CubeWithTextureData(GL gl, Camera camera) {
    this.gl = gl;
    this.camera = camera;

    Shaders = new ShaderProgram(gl, vertexShaderName, fragmentShaderName);
    texture = new Simple2DTexture(gl, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "wooden_crate.jpg"));
  }

  public override void Dispose() {
    Shaders.Dispose();
    texture.Dispose();
  }

  public override void Draw(double deltaTime, double absoluteTime) {
    var rotationAngle = MathHelper.DegreesToRadians((float) (absoluteTime * 100));
    var modelMatrix = Matrix4x4.CreateRotationX(rotationAngle) * Matrix4x4.CreateRotationY(rotationAngle);

    texture.Bind(TextureUnit.Texture0);
    Shaders.Use();
    Shaders.SetUniform("uTexture", 0);
    Shaders.SetUniform("uModel", modelMatrix);
    Shaders.SetUniform("uView", camera.ViewMatrix);
    Shaders.SetUniform("uProjection", camera.ProjectionMatrix);
    //ApplyTransformationIfNeeded();
    gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
  }
}