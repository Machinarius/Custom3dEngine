using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class OpenGLContext : IGraphicsContext, IDisposable
{
    public GL GL { get; }
    
    public OpenGLContext(GL gl)
    {
        GL = gl ?? throw new ArgumentNullException(nameof(gl));
    }
    
    public void Dispose()
    {
        GL?.Dispose();
    }
}