#if UNITY_EDITOR
using UnityEngine;

public class TransformData
{
    public Vector3 Positions;
    public Quaternion Rotation;
    public Vector3 Scale;

    public TransformData(Vector3 positions, Quaternion rotation, Vector3 scale)
    {
        Positions = positions;
        Rotation = rotation;
        Scale = scale;
    }

    public override string ToString()
    {
        var debug = $"Position {Positions} Scale {Scale} Rotation {Rotation}";
        return debug;
    }
}
#endif