using Machinarius.Custom3dEngine.DebugUtils;
using Silk.NET.OpenGL;

namespace Machinarius.Custom3dEngine.GLAbstractions;

public class Simple2DTexture : IDisposable {
  private readonly uint handle;
  private readonly GL gl;

  public string? FilePath;

  public Simple2DTexture(GL gl, string filePath) {
    this.gl = gl;
    FilePath = filePath;

    handle = gl.GenTexture();
    Bind();
    LoadTextureFromFile(filePath);
  }

  // This will mostly be used for textures we generate ourselves in code
  // Those will generally encode data for shaders to use
  public Simple2DTexture(GL gl, ReadOnlySpan<byte> rawTextureData, uint width, uint height) {
    this.gl = gl;

    handle = gl.GenTexture();
    Bind();
    LoadTextureFromRawData(rawTextureData, width, height);
  }
  public Simple2DTexture(GL gl, Image<Rgba32> sourceImage, string? debugName = null) {
    this.gl = gl;

    handle = gl.GenTexture();
    Bind();
    if (!string.IsNullOrEmpty(debugName)) {
      gl.TagAsset(handle, ObjectIdentifier.Texture, debugName);
    }
    
    LoadImageToTexture(sourceImage);
    SetParameters();
  }

  public void Bind(TextureUnit textureSlot = TextureUnit.Texture0) {
    gl.ActiveTexture(textureSlot);
    gl.BindTexture(TextureTarget.Texture2D, handle);
  }

  public void Unbind() {
    gl.BindTexture(TextureTarget.Texture2D, 0);
  }

  private void LoadTextureFromFile(string filePath) {
    using var image = Image.Load<Rgba32>(filePath);
    LoadImageToTexture(image);
    SetParameters();
  }

  private unsafe void LoadImageToTexture(Image<Rgba32> image) {
    //Reserve enough memory from the gpu for the whole image
    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)image.Width, (uint)image.Height, 0,
      PixelFormat.Rgba, PixelType.UnsignedByte, null);
    Console.WriteLine($"Reserved memory for a {image.Width}x{image.Height} texture.");
    image.ProcessPixelRows(dataReader => {
      for (var y = dataReader.Height - 1; y >= 0; y--) {
        var yOffset = dataReader.Height - y - 1;
        //ImageSharp 2 does not store images in contiguous memory by default, so we must send the image row by row
        fixed (void* data = dataReader.GetRowSpan(y)) {
          //Loading the row of pixel data
          gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, yOffset, (uint)dataReader.Width, 1, PixelFormat.Rgba,
            PixelType.UnsignedByte, data);
          gl.EnsureCallSucceeded();
        }
      }
    });
  }

  private unsafe void LoadTextureFromRawData(ReadOnlySpan<byte> rawTextureData, uint width, uint height) {
    fixed (void* dataPointer = &rawTextureData[0]) {
      gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dataPointer);
    }
    SetParameters();
  }

  private void SetParameters() {
    //Setting some texture parameters so the texture behaves as expected.
    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
    //Generating mipmaps.
    gl.GenerateMipmap(TextureTarget.Texture2D);
    gl.EnsureCallSucceeded();
  }

  public void Dispose() {
    gl.DeleteTexture(handle);
  }
}
