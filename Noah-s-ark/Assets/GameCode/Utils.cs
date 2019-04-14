using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class Utils
{
    public static float Float3MagnitudeSq(float3 value)
    {
        return value.x * value.x + value.y * value.y + value.z * value.z;
    }

    public static float Float3Magnitude(float3 value)
    {
        return math.sqrt(Float3MagnitudeSq(value));
    }

    public static bool IsCollidingCirclePoint(float3 pos1, float radius1, float3 pos2)
    {
        var diffVector = pos1 - pos2;
        var distanceSq = Utils.Float3MagnitudeSq(diffVector);
        return (distanceSq < radius1 * radius1);
    }

    public static bool IsCollidingCircleCircle(float3 pos1, float radius1, float3 pos2, float radius2)
    {
        return math.pow(pos2.x - pos1.x, 2) + math.pow(pos2.z - pos1.z, 2) <= math.pow(radius1 + radius2,2);
    }
}
