using System.Numerics;

namespace Machinarius.Custom3dEngine.Helpers;

public static class TransformationMath
{
    public static Matrix4x4 BuildModelMatrix(Vector3 position, float scale, Quaternion rotation)
    {
        return Matrix4x4.Identity * 
            Matrix4x4.CreateFromQuaternion(rotation) * 
            Matrix4x4.CreateScale(scale) * 
            Matrix4x4.CreateTranslation(position);
    }
    
    public static Vector3 ApplyTransformation(Vector3 point, Matrix4x4 transformMatrix)
    {
        return Vector3.Transform(point, transformMatrix);
    }
    
    public static bool IsValidScale(float scale)
    {
        return scale > 0 && !float.IsNaN(scale) && !float.IsInfinity(scale);
    }
    
    public static bool IsValidPosition(Vector3 position)
    {
        return !float.IsNaN(position.X) && !float.IsNaN(position.Y) && !float.IsNaN(position.Z) &&
               !float.IsInfinity(position.X) && !float.IsInfinity(position.Y) && !float.IsInfinity(position.Z);
    }
}