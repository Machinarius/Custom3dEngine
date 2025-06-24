# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a custom 3D engine built in C# using Silk.NET for OpenGL bindings. The engine uses an Entity-Component-System-like architecture with a scene graph and supports 3D model loading, lighting, texturing, and basic rendering.

## Development Commands

### Build and Run
- `dotnet build` - Build the project
- `dotnet run` - Build and run the application
- `dotnet clean` - Clean build artifacts
- `dotnet restore` - Restore NuGet packages

### Development
- `dotnet watch run` - Run with file watching for hot reload during development

## Architecture Overview

### Core Structure
- **Program.cs**: Entry point that creates a window and starts the RenderConfigurator
- **RenderConfigurator**: Sets up the rendering pipeline and initializes the RenderOrchestrator
- **RenderOrchestrator**: Main rendering loop and event handling (GLAbstractions/RenderOrchestrator.cs)
- **SceneBuilder**: Constructs scenes with objects, lighting, and materials (GLAbstractions/SceneBuilder.cs)

### Key Components

#### GLAbstractions/
OpenGL wrapper layer providing higher-level abstractions:
- **BufferedMesh**: GPU buffer management for mesh data
- **ShaderProgram**: GLSL shader compilation and management
- **VertexArrayObject/BufferObject**: OpenGL state management
- **Simple2DTexture**: Texture loading and binding

#### Entities/
Scene graph and object system:
- **Scene**: Container for all scene objects with a camera
- **SceneObject**: Individual renderable objects with position, scale, rotation
- **Camera**: View and projection matrix management
- **Attributes/**: Component system for materials and lighting (SimpleMaterial, SpecularWithTextureMaterial, LitByEmmisive)
- **Behaviors/**: Transformation behaviors like RotationOnXY

#### Meshes/
Geometry definitions:
- **Model**: 3D model loader using Assimp (supports .obj files)
- **Cube variants**: Different cube meshes with normals, UVs, colors
- **Quad variants**: 2D quads for UI elements

### Rendering Pipeline
1. SceneBuilder creates a Scene with Camera and SceneObjects
2. Each SceneObject has a BufferedMesh (geometry) and ShaderProgram (materials)
3. Scene.Draw() iterates through objects, calling SceneObject.Draw()
4. Objects apply their Attributes (materials, lighting) and render

### Shader System
Shaders are in the Shaders/ directory:
- Vertex shaders: Transform vertices (MVP matrices, normals, UVs)
- Fragment shaders: Handle lighting, texturing, and color output
- Naming convention: "IdentityWithMVP.vert" describes what the shader does

### Asset Pipeline
- Assets/ directory contains textures and 3D models
- global.json specifies .NET 7.0 SDK
- Project file copies shaders and assets to output directory

## Dependencies
- **Silk.NET**: OpenGL bindings and windowing
- **Silk.NET.Assimp**: 3D model loading
- **SixLabors.ImageSharp**: Image/texture loading
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **Target Framework**: .NET 7.0

## Dependency Injection
The engine uses Microsoft's DI container for managing dependencies:
- **IGraphicsContext**: OpenGL context abstraction
- **IResourceFactory**: Factory for creating meshes, shaders, textures
- **SceneObjectFactory**: Factory for creating scene objects with proper dependencies
- **SceneBuilder**: Creates scenes using the factory pattern

Services are registered in RenderOrchestrator constructor and resolved automatically.