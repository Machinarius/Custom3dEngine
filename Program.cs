using Machinarius.Custom3dEngine.Meshes;
using Machinarius.Custom3dEngine.Shaders;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Machinarius.Custom3dEngine;

public class Program {
  public static void Main() {
    GL? glContext = null;
    IInputContext? inputContext = null;

    var options = WindowOptions.Default;
    options.Title = "My 3d engine";

    UploadedMesh? uploadedQuad = null;
    uint shaderProgramId = 0;

    using var window = Window.Create(options);
    window.Load += () => {
      glContext = window.CreateOpenGL();
      inputContext = window.CreateInput();

      uploadedQuad = MeshUploader.UploadMeshDataToGpu(glContext, new Quad());
      shaderProgramId = ProgramFactory.FromShaderFiles(glContext, "IdentityVertex.vert", "MediumBlue.frag");

      unsafe {
        glContext.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
      }
      glContext.EnableVertexAttribArray(0);
    };

    window.FramebufferResize += size => {
      glContext?.Viewport(size);
    };

    window.Render += deltaTime => {
      glContext?.ClearColor(System.Drawing.Color.Wheat);
      glContext?.Clear(ClearBufferMask.ColorBufferBit);

      if (uploadedQuad == null) {
        throw new InvalidOperationException("The Quad mesh has not been uploaded to the GPU yet!");
      }

      glContext?.BindVertexArray(uploadedQuad.VertexArrayObjectId);
      glContext?.UseProgram(shaderProgramId);

      unsafe {
        glContext?.DrawElements(PrimitiveType.Triangles, (uint) uploadedQuad.Indices.Length, DrawElementsType.UnsignedInt, null);
      }

      if (!ShouldGameStillRun(inputContext)) {
        window.Close();
      }
    };

    window.Closing += () => {
      uploadedQuad?.Destroy(glContext);
      glContext?.DeleteProgram(shaderProgramId);

      glContext?.Dispose();
      inputContext?.Dispose();
    };

    window.Run();
  }

  static private bool ShouldGameStillRun(IInputContext? inputContext) {
    if (inputContext == null || inputContext.Keyboards.Count < 1) {
      // Can't ESC out when there are no keyboards ¯\_(ツ)_/¯
      return true;
    }

    if (inputContext.Keyboards[0].IsKeyPressed(Key.Escape)) {
      return false;
    }

    return true;
  }
}
