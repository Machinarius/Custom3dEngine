using Machinarius.Custom3dEngine.Meshes;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public interface IResourceFactory
{
    BufferedMesh CreateMesh(IMesh sourceMesh, string name);
    ShaderProgram CreateShader(string vertexFile, string fragmentFile);
    Simple2DTexture CreateTexture(string filePath);
}