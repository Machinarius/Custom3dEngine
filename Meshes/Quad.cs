using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.Meshes;

public class Quad : BaseMesh {
  private const string vertexShaderName = "IdentityVertex.vert";

  private const string fragmentShaderName = "MediumBlue.frag";
  
  public override VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 3 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 3, 0),
  };

  // Without any projection or model transformations these are effectively clip-space
  // coordinates in the range [-1, 1]. For example 1 in the X axis is the right-most pixel
  // of the view-port, and -1 the left-most.
  public override float[] Vertices => new[] {
      0.5f,  0.5f, 0.0f,
      0.5f, -0.5f, 0.0f,
     -0.5f, -0.5f, 0.0f,
     -0.5f,  0.5f, 0.5f
  };

  public override uint[] Indices => new uint[] {
    0, 1, 3,
    1, 2, 3
  };

  public override Transformation? Transformation { get; set; }
  protected override ShaderProgram Shaders { get; }

  private readonly GL gl;

  public Quad(GL gl) {
    this.gl = gl;
    Shaders = new ShaderProgram(gl, vertexShaderName, fragmentShaderName);
  }

  public unsafe override void Draw(double deltaTime, double absoluteTime) {
    Shaders.Use();
    ApplyTransformationIfNeeded();
    // This simple quad has no uniforms
    gl.DrawElements(PrimitiveType.Triangles, (uint) Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public override void Dispose() {
    Shaders.Dispose();
  }
}