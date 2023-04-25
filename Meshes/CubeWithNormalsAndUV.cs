using Machinarius.Custom3dEngine.DebugUtils;
using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.OpenGL;
using System.Text.Json;

namespace Machinarius.Custom3dEngine.Meshes;

public class CubeWithNormalsAndUV : IMesh {
  public VertexAttributeDescriptor[] Attributes => new [] {
    // 3 floats for XYZ coordinates every 8 elements, starting from 0
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 8, 0, VertexAttributePayloadType.Position),
    // 3 floats for XYZ of the normal every 6 elements, starting from 3
    new VertexAttributeDescriptor(3, VertexAttribPointerType.Float, 8, 3, VertexAttributePayloadType.Normal),
    // 2 floats for UV coords every 8 elements, starting from 6
    new VertexAttributeDescriptor(2, VertexAttribPointerType.Float, 8, 6, VertexAttributePayloadType.TextureCoordinates)
  };

  public float[] Vertices => new [] {
    1f,1f,-1f,		0f,1f,0f,		  1f,1f, // Vertex 0, Facing Y 
    -1f,1f,-1f,		0f,1f,0f,		  0f,1f, // Vertex 1, Facing Y
    -1f,1f,1f,		0f,1f,0f,		  0f,0f, // Vertex 2, Facing Y
    1f,1f,1f,		  0f,1f,0f,		  1f,0f, // Vertex 3, Facing Y
    1f,-1f,1f,		0f,0f,1f,		  1f,0f, // Vertex 4, Facing Z
    1f,1f,1f,		  0f,0f,1f,		  1f,1f, // Vertex 3, Facing Z
    -1f,1f,1f,		0f,-0f,1f,		0f,1f, // Vertex 2, Facing Z
    -1f,-1f,1f,		-0f,-0f,1f,		0f,0f, // Vertex 5, Facing Z
    -1f,-1f,1f,		-1f,-0f,0f,		0f,0f, // Vertex 5, Facing -X
    -1f,1f,1f,		-1f,0f,0f,		0f,1f, // Vertex 2, Facing -X
    -1f,1f,-1f,		-1f,0f,0f,		1f,1f, // Vertex 1, Facing -X
    -1f,-1f,-1f,	-1f,0f,0f,		1f,0f, // Vertex 6, Facing -X
    -1f,-1f,-1f,	0f,-1f,0f,		0f,1f, // Vertex 6, Facing -Y
    1f,-1f,-1f,		0f,-1f,0f,		1f,1f, // Vertex 7, Facing -Y
    1f,-1f,1f,		0f,-1f,0f,		1f,0f, // Vertex 4, Facing -Y
    -1f,-1f,1f,		0f,-1f,0f,		0f,0f, // Vertex 5, Facing -Y
    1f,-1f,-1f,		1f,0f,0f,		  1f,0f, // Vertex 7, Facing X
    1f,1f,-1f,		1f,0f,0f,		  1f,1f, // Vertex 0, Facing X
    1f,1f,1f,		  1f,0f,0f,		  0f,1f, // Vertex 3, Facing X
    1f,-1f,1f,		1f,0f,0f,		  0f,0f, // Vertex 4, Facing X
    -1f,-1f,-1f,	0f,0f,-1f,		0f,0f, // Vertex 6, Facing -Z
    -1f,1f,-1f,		0f,0f,-1f,		0f,1f, // Vertex 1, Facing -Z
    1f,1f,-1f,		0f,0f,-1f,		1f,1f, // Vertex 0, Facing -Z
    1f,-1f,-1f,   0f,0f,-1f,    1f,0f  // Vertex 7, Facing -Z
  };

  public uint[] Indices => new uint[] { 
    0,1,2,    // +Y Face, -Z Triangle, v0, v1, v2
    0,2,3,    // +Y Face, +Z Triangle, v0, v2, v3
    4,5,6,    // +Z Face, -Y Triangle, v4, v3, v2
    4,6,7,    // +Z Face, +Y Triangle, v4, v2, v5
    8,9,10,   // -X Face, -Y Triangle, v5, v2, v1
    8,10,11,  // -X Face, +Y Triangle, v5, v1, v6
    12,13,14, // -Y Face, +X Triangle, v6, v7, v4
    12,14,15, // -Y Face, -X Triangle, v6, v4, v5
    16,17,18, // +X Face, +Y Triangle, v7, v0, v3
    16,18,19, // +X Face, -Y Triangle, v7, v3, v4
    20,21,22, // -Z Face, -X Triangle, v6, v1, v0
    20,22,23  // -Z Face, +X Triangle, v6, v0, v7
  };

  public Simple2DTexture? DiffuseTexture => texture;
  public Simple2DTexture? SpecularTexture => null;

  private readonly GL gl;
  private readonly Simple2DTexture texture;
  private readonly Simple2DTexture specularMap;

  public CubeWithNormalsAndUV(GL gl) {
    this.gl = gl;
    texture = new Simple2DTexture(gl, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "silkBoxed.png"));
    specularMap = new Simple2DTexture(gl, Path.Combine(Directory.GetCurrentDirectory(), "Assets", "silkSpecular.png"));
  }

  public unsafe void Draw() {
    texture.Bind(TextureUnit.Texture0);
    specularMap.Bind(TextureUnit.Texture1);
    
    //gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)Vertices.Length);
    gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
  }

  public void Dispose() {}
}