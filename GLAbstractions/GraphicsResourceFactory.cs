using Machinarius.Custom3dEngine.Meshes;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class GraphicsResourceFactory : IResourceFactory {
  private readonly IGraphicsContext _context;

  public GraphicsResourceFactory(IGraphicsContext context) {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public BufferedMesh CreateMesh(IMesh sourceMesh, string name) {
    return new BufferedMesh(_context.GL, sourceMesh, name);
  }

  public ShaderProgram CreateShader(string vertexFile, string fragmentFile) {
    return new ShaderProgram(_context.GL, vertexFile, fragmentFile);
  }

  public Simple2DTexture CreateTexture(string filePath) {
    return new Simple2DTexture(_context.GL, filePath);
  }
}