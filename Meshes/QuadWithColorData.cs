using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class QuadWithColorData : BaseMesh {

  private const string vertexShaderName = "IdentityWithColor.vert";

  private const string fragmentShaderName = "ArgumentColor.frag";

  public override VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 7 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 7, 0),
    // 4 floats for RGBA color every 7 elements, starting from the third element
    new VertexAttributeDescriptor(4, VertexAttribPointerType.Float, 7, 3),
  };
  
  public override float[] Vertices => new[] {
    //X     Y      Z     R  G  B  A
      0.5f,  0.5f, 0.0f, 1, 0, 0, 1,
      0.5f, -0.5f, 0.0f, 0, 0, 0, 1,
     -0.5f, -0.5f, 0.0f, 0, 0, 1, 1,
     -0.5f,  0.5f, 0.5f, 0, 0, 0, 1
  };

  public override uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };

  public override Transformation? Transformation { get; set; }
  protected override ShaderProgram Shaders { get; }

  private readonly GL gl;

  public QuadWithColorData(GL gl) {
    this.gl = gl;
    Shaders = new ShaderProgram(gl, vertexShaderName, fragmentShaderName);
  }

  public unsafe override void Draw(double deltaTime, double absoluteTime) {
    Shaders.Use();
    ApplyTransformationIfNeeded();
    Shaders.SetUniform("uBlue", (float) Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));
    gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public override void Dispose() {
    Shaders.Dispose();
  }
}