using Machinarius.Custom3dEngine.Meshes;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Collections.Generic;
using System.Numerics;

namespace Machinarius.Custom3dEngine.GLAbstractions; 

public class TextPainter : IDisposable {
  private readonly IWindow window;
  private readonly GL gl;
  private readonly Font montserratFont;
  
  private ShaderProgram textShader;
  private BufferedMesh glyphTarget;
  
  public TextPainter(IWindow window, GL gl) {
    this.window = window ?? throw new ArgumentNullException(nameof(window));
    this.gl = gl ?? throw new ArgumentNullException(nameof(gl));
    
    window.FramebufferResize += OnFramebufferResize;
    
    var fonts = new FontCollection();
    fonts.Add("Assets/Fonts/Montserrat/Montserrat-VariableFont.ttf");
    montserratFont = fonts.Get("Montserrat Thin").CreateFont(10, FontStyle.Regular);
    
    textOptions = BuildTextOptions(window.Size);
    projectionMatrix = BuildProjectionMatrix(window.Size);
    
    textShader = new ShaderProgram(gl, "HUDElement.vert", "BasicTextureWithAlphaDiscard.frag");
    glyphTarget = new BufferedMesh(gl, new Quad(gl));
    glyphTarget.ActivateVertexAttributes();
  }

  private void OnFramebufferResize(Vector2D<int> framebufferSize) {
    textOptions = BuildTextOptions(framebufferSize);
    projectionMatrix = BuildProjectionMatrix(framebufferSize);
  }

  private TextOptions BuildTextOptions(Vector2D<int> bounds) {
    return new TextOptions(montserratFont) {
      HorizontalAlignment = HorizontalAlignment.Left,
      VerticalAlignment = VerticalAlignment.Top,
      WrappingLength = bounds.X
    };
  }
  
  private Matrix4x4 BuildProjectionMatrix(Vector2D<int> bounds) {
    return Matrix4x4.CreateOrthographic(
      bounds.X, bounds.Y, -1, 1
    );
  }

  private TextOptions textOptions;
  private Matrix4x4 projectionMatrix;
  private Dictionary<char, Simple2DTexture> glyphCache = new();

  public void RenderText(
    string text,
    Vector2D<float> positionInScreen
  ) {
    var glyphs = TextBuilder.GenerateGlyphs(text, textOptions).ToArray();
      
    glyphTarget.Bind();
    textShader.Use();
    textShader.SetUniform("uProjection", projectionMatrix);
    
    for (var i = 0; i < text.Length; i++) {
      var character = text[i];
      if (character == ' ') {
        continue; // TODO: There are more non-printable characters!
      }
      var glyph = glyphs[i];

      if (!glyphCache.TryGetValue(character, out var glyphTexture)) {
        var normalizedWidth = (int)Math.Ceiling(glyph.Bounds.Width);
        var normalizedHeight = (int)Math.Ceiling(glyph.Bounds.Height);
        Console.WriteLine($"Generating a {normalizedWidth}x{normalizedHeight} texture for {character}");
        
        using var glyphImage = new Image<Rgba32>(normalizedWidth, normalizedHeight);
        glyphImage.Mutate(ctx => ctx.Fill(Color.White, glyph));
        using var debugOutput = File.Create($"glyph{character}.png");
        glyphImage.SaveAsPng(debugOutput);
        
        glyphTexture = new Simple2DTexture(gl, glyphImage);
        glyphCache.Add(character, glyphTexture); 
      }
      
      // The glyphTarget mesh renders as a 1px square when ortho-projected
      // so we need to scale it up to the size of the glyph
      var modelMatrix = Matrix4x4.CreateScale(glyph.Bounds.Width * 3, glyph.Bounds.Height * 3, 1);
      // Now we need to move the glyph to the correct position in the screen
      modelMatrix *= Matrix4x4.CreateTranslation(
        positionInScreen.X + glyph.Bounds.X, 
        positionInScreen.Y + glyph.Bounds.Y, 
        0
      );
      textShader.SetUniform("uModel", modelMatrix);
      glyphTexture.Bind();
      glyphTarget.Draw();
      glyphTexture.Unbind();
    }
    glyphTarget.Unbind();
  }

  public void Dispose() {
    window.FramebufferResize -= OnFramebufferResize;
  }
}