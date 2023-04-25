﻿using Machinarius.Custom3dEngine.GLAbstractions.Rendering;
using Silk.NET.Windowing;

namespace Machinarius.Custom3dEngine;

public class Program {
  public static void Main() {
    var options = WindowOptions.Default;
    options.Title = "Machinarius 3D Engine";
    
    using var window = Window.Create(options);
    var renderer = new RenderConfigurator(window);
    renderer.Run();
  }
}
