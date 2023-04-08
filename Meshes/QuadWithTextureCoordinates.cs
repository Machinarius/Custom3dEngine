using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class QuadWithTextureCoordinates : BaseMesh {
  private readonly Simple2DTexture texture;

  private const string vertexShaderName = "TransformationWithUv.vert";

  private const string fragmentShaderName = "BasicTextureWithAlphaDiscard.frag";

  public override VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 5 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 5, 0),
    // 2 floats for UV coordinates every 5 elements, starting from the third element
    new VertexAttributeDescriptor(4, VertexAttribPointerType.Float, 5, 3),
  };
  
  public override float[] Vertices => new[] {
    //X     Y      Z     U  V
      0.5f,  0.5f, 0.0f, 1, 0, 
      0.5f, -0.5f, 0.0f, 1, 1, 
     -0.5f, -0.5f, 0.0f, 0, 1, 
     -0.5f,  0.5f, 0.5f, 0, 0
  };

  public override uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };
  public override Transformation? Transformation { get; set; }
  protected override ShaderProgram Shaders { get; }

  public QuadWithTextureCoordinates(GL gl) {
    Shaders = new ShaderProgram(gl, vertexShaderName, fragmentShaderName);
    texture = new Simple2DTexture(gl, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "silk.png"));
  }

  public override void PrepareForDrawing() {
    texture.Bind(TextureUnit.Texture0);
    Shaders.Use();
    Shaders.SetUniform("uTexture", 0);
    ApplyTransformationIfNeeded();
  }

  public override void Dispose() {
    Shaders.Dispose();
    texture.Dispose();
  }
}
