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

    public static bool IsCollidingCirclePoint(float3 circlePosition, float circleRadius, float3 point)
    {
        return IsCollidingCircleCircle(circlePosition, circleRadius, point, 0);
    }

    public static bool IsCollidingCircleCircle(float3 pos1, float radius1, float3 pos2, float radius2)
    {
        var diffVector = pos2 - pos1;
        diffVector.y = 0; //Circle is 2d
        var distanceSq = Utils.Float3MagnitudeSq(diffVector);
        return (distanceSq < (radius1 + radius2) * (radius1 + radius2));
    }
}
