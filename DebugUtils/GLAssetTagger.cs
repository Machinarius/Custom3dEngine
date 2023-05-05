using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.DebugUtils; 

public static class GLAssetTagger {
  public static void TagAsset(this GL gl, uint handle, ObjectIdentifier identifier, string tag) {
    var maxLength = gl.GetInteger(GLEnum.MaxLabelLength);
    var limitedTag = tag;
    if (limitedTag.Length > maxLength) {
      limitedTag = limitedTag[..maxLength];
    }
    gl.ObjectLabel(identifier, handle, (uint)limitedTag.Length, limitedTag);
    gl.EnsureCallSucceeded();
    Console.WriteLine($"Tagged {identifier} #{handle} as {tag}");
  }
}