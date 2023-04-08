using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class QuadWithTextureCoordinates : IMesh {
  private readonly ShaderProgram shaders;
  private readonly Simple2DTexture texture;

  private const string vertexShaderName = "IdentityWithUv.vert";

  private const string fragmentShaderName = "BasicTextureWithAlphaDiscard.frag";

  public VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 5 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 5, 0),
    // 2 floats for UV coordinates every 5 elements, starting from the third element
    new VertexAttributeDescriptor(4, VertexAttribPointerType.Float, 5, 3),
  };
  
  public float[] Vertices => new[] {
    //X     Y      Z     U  V
      0.5f,  0.5f, 0.0f, 1, 0, 
      0.5f, -0.5f, 0.0f, 1, 1, 
     -0.5f, -0.5f, 0.0f, 0, 1, 
     -0.5f,  0.5f, 0.5f, 0, 0
  };

  public uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };

  public QuadWithTextureCoordinates(GL gl) {
    shaders = new ShaderProgram(gl, vertexShaderName, fragmentShaderName);
    texture = new Simple2DTexture(gl, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "silk.png"));
  }

  public void ConfigureUniforms() {
    texture.Bind(TextureUnit.Texture0);
    shaders.Use();
    shaders.SetUniform("uTexture", 0);
  }

  public void Dispose() {
    shaders.Dispose();
    texture.Dispose();
  }
}
