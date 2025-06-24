using System;

namespace Machinarius.Custom3dEngine.Generators;

/// <summary>
/// Marks a class for automatic GL error checking injection.
/// All GL method calls in the class will automatically have error checking added.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GLErrorCheckedAttribute : Attribute {
    /// <summary>
    /// If true, error checking is only added in DEBUG builds. Default: true
    /// </summary>
    public bool DebugOnly { get; set; } = true;

    /// <summary>
    /// If true, batches error checks at the end of methods instead of after each call. Default: false
    /// </summary>
    public bool BatchCheck { get; set; } = false;

    /// <summary>
    /// Array of GL method names to exclude from error checking.
    /// </summary>
    public string[]? ExcludeMethods { get; set; }
}

/// <summary>
/// Marks a method to exclude from GL error checking injection.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class SkipGLErrorCheckAttribute : Attribute {
}

/// <summary>
/// Assembly-level attribute to enable automatic GL error checking for all classes with GL usage.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class GLErrorCheckingAttribute : Attribute {
    /// <summary>
    /// If true, error checking is only added in DEBUG builds. Default: true
    /// </summary>
    public bool DebugOnly { get; set; } = true;

    /// <summary>
    /// If true, batches error checks at the end of methods instead of after each call. Default: false
    /// </summary>
    public bool BatchCheck { get; set; } = false;

    /// <summary>
    /// Array of GL method names to exclude from error checking.
    /// </summary>
    public string[]? ExcludeMethods { get; set; }
}