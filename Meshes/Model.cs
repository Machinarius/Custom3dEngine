using Machinarius.Custom3dEngine.GLAbstractions;
using Silk.NET.Assimp;
using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using System.Collections;
using System.Numerics;
using System.Text.Json;
using AssimpMesh = Silk.NET.Assimp.Mesh;

namespace Machinarius.Custom3dEngine.Meshes;

public class Model {
  private readonly GL gl;
  private readonly Assimp assimp;

  private readonly List<IMesh> meshes = new();
  public IMesh[] Meshes => meshes.ToArray();

  public Model(GL gl, string path) {
    if (string.IsNullOrEmpty(path)) {
      throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
    }

    this.gl = gl;

    assimp = Assimp.GetApi();
    LoadModelData(path);
  }

  private unsafe void LoadModelData(string path) {
    var loadFlags = (uint)(
      PostProcessSteps.Triangulate
    );
    var scene = assimp.ImportFile(path, loadFlags);

    if (scene == null || scene->MRootNode == null || scene->MFlags == Assimp.SceneFlagsIncomplete) { 
      var errorMessage = SilkMarshal.PtrToString((nint)assimp.GetErrorString());
      throw new InvalidOperationException($"Could not load model from '{path}'\n{errorMessage}");
    }

    VisitNode(scene->MRootNode, scene);
  }

  private unsafe void VisitNode(Node* mRootNode, Scene* scene) {
    for (var i = 0; i < mRootNode->MNumMeshes; i++) {
      var mesh = scene->MMeshes[mRootNode->MMeshes[i]];
      VisitMesh(mesh, scene);
    }

    for (var i = 0; i < mRootNode->MNumChildren; i++) {
      var child = mRootNode->MChildren[i];
      VisitNode(child, scene);
    }
  }

  private unsafe void VisitMesh(AssimpMesh* mesh, Scene* scene) {
    var vertices = new Vertex[mesh->MNumVertices];

    for (var i = 0; i < mesh->MNumVertices; i++) {
      Vector3? normals = null;
      Vector2? uvs = null;

      var position = new Vector3(mesh->MVertices[i].X, mesh->MVertices[i].Y, mesh->MVertices[i].Z);
      if (mesh->MNormals != null) {
        normals = new Vector3(mesh->MNormals[i].X, mesh->MNormals[i].Y, mesh->MNormals[i].Z);
      }

      // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
      // use models where a vertex can have multiple texture coordinates so we always take the first set (0).
      if (mesh->MTextureCoords[0] != null) {
        uvs = new Vector2(mesh->MTextureCoords[0][i].X, mesh->MTextureCoords[0][i].Y);
      }

      vertices[i] = new Vertex {
        Position = position,
        Normal = normals,
        UvCoordinates = uvs,
      };
    }

    var indices = new List<uint>();
    var faceCount = mesh->MNumFaces;
    for (var i = 0; i < faceCount; i++) {
      var face = mesh->MFaces[i];
      var indexCount = face.MNumIndices;
      if (indexCount != 3) {
        throw new InvalidOperationException($"Face {i} has {indexCount} indices. Only faces with 3 indices are supported.");
      }

      indices.Add(face.MIndices[0]);
      indices.Add(face.MIndices[1]);
      indices.Add(face.MIndices[2]);
    }

    var material = scene->MMaterials[mesh->MMaterialIndex];
    var diffuseTextures = LoadMaterialTextures(material, TextureType.Diffuse);
    var specularTextures = LoadMaterialTextures(material, TextureType.Specular);
    // var normalTextures = LoadMaterialTextures(material, TextureType.Normals);
    // var heightTextures = LoadMaterialTextures(material, TextureType.Height);

    var diffuse = diffuseTextures.FirstOrDefault();
    var specular = specularTextures.FirstOrDefault();
    
    var meshData = BuildMeshData(vertices);
    var result = new DataMesh(gl, meshData.Attributes, meshData.Vertices, indices.ToArray(), diffuse, specular);
    meshes.Add(result);
  }

  private readonly List<Simple2DTexture> textureCache = new();

  private unsafe Simple2DTexture[] LoadMaterialTextures(Material* material, TextureType type) {
    var textureCount = assimp.GetMaterialTextureCount(material, type);
    if (textureCount == 0) {
      return Array.Empty<Simple2DTexture>();
    }

    var textures = new Simple2DTexture[textureCount];
    for (var i = 0; i < textureCount; i++) {
      AssimpString rawTexturePath;
      assimp.GetMaterialTexture(material, type, (uint)i, &rawTexturePath, null, null, null, null, null, null);

      var texturePath = ResolveTexturePath(rawTexturePath.ToString());
      var cachedTexture = textureCache.FirstOrDefault(t => t.FilePath == texturePath);
      if (cachedTexture != null) {
        textures[i] = cachedTexture;
        continue;
      }

      var texture = new Simple2DTexture(gl, texturePath);
      textureCache.Add(texture);
      textures[i] = texture;
    }

    return textures;
  }

  private record MeshData(float[] Vertices, VertexAttributeDescriptor[] Attributes);
  
  static private MeshData BuildMeshData(Vertex[] vertices) {
    uint attributeStride = 3;

    // Assuming all vertices have the same attributes
    if (vertices[0].Normal.HasValue) {
      attributeStride += 3;
    }

    if (vertices[0].UvCoordinates.HasValue) {
      attributeStride += 2;
    }

    var attributes = new List<VertexAttributeDescriptor>() {
      new(3, VertexAttribPointerType.Float, attributeStride, 0, VertexAttributePayloadType.Position),
    };

    if (vertices[0].Normal.HasValue) {
      attributes.Add(new (3, VertexAttribPointerType.Float, attributeStride, 3, VertexAttributePayloadType.Normal));
    }

    if (vertices[0].UvCoordinates.HasValue) {
      var offset = vertices[0].Normal.HasValue ? 6 : 3;
      attributes.Add(new (2, VertexAttribPointerType.Float, attributeStride, offset, VertexAttributePayloadType.TextureCoordinates));
    }

    var vertexData = new float[vertices.Length * attributeStride];
    for (var i = 0; i < vertices.Length; i++) {
      var vertex = vertices[i];
      var offset = i * attributeStride;
      vertexData[offset++] = vertex.Position.X;
      vertexData[offset++] = vertex.Position.Y;
      vertexData[offset++] = vertex.Position.Z;

      if (vertex.Normal.HasValue) {
        vertexData[offset++] = vertex.Normal.Value.X;
        vertexData[offset++] = vertex.Normal.Value.Y;
        vertexData[offset++] = vertex.Normal.Value.Z;
      }

      if (vertex.UvCoordinates.HasValue) {
        vertexData[offset++] = vertex.UvCoordinates.Value.X;
        vertexData[offset] = vertex.UvCoordinates.Value.Y;
      }
    }

    return new MeshData(vertexData, attributes.ToArray());
  }

  static private readonly string textureDirectory = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Assets"
  );

  static private string ResolveTexturePath(string texturePath) {
    string resolvedPath;

    if (Path.IsPathFullyQualified(texturePath)) {
      resolvedPath = texturePath;
    } else {
      resolvedPath = Path.Combine(textureDirectory, texturePath);
    }

    if (!System.IO.File.Exists(resolvedPath)) {
      throw new InvalidOperationException($"Could not find texture '{texturePath}'");
    }
    
    return resolvedPath;
  }
}