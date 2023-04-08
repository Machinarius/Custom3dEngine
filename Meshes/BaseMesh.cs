using Machinarius.Custom3dEngine.GLAbstractions;

namespace Machinarius.Custom3dEngine.Meshes;

public abstract class BaseMesh : IMesh {
  public abstract VertexAttributeDescriptor[] Attributes { get; }

  public abstract float[] Vertices { get; }

  public abstract uint[] Indices { get; }
  public abstract Transformation? Transformation { get; set; }

  public abstract void Dispose();

  protected abstract ShaderProgram Shaders { get; }

  public abstract void PrepareForDrawing();

  protected void ApplyTransformationIfNeeded() {
    if (Transformation == null) {
      return;
    }

    if (!Shaders.HasUniform("uModelMatrix")) {
      throw new InvalidOperationException($"Cannot apply transformations to this '{GetType().Name}' mesh, the 'uModelMatrix' uniform is not present in the shader");
    }

    Shaders.SetUniform("uModelMatrix", Transformation.ViewMatrix);
  }
}
