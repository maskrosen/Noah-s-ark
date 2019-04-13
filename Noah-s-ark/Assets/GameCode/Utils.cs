﻿using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class Utils
{

    public static float Float3Magnitude(float3 value)
    {
        return math.sqrt(value.x*value.x + value.y*value.y + value.z*value.z);
    }

}
