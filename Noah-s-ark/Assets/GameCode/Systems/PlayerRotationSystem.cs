using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class PlayerRotationSystem : ComponentSystem
{

    public struct RotationData
    {
        public readonly int Length;
        public ComponentDataArray<Rotation> Rotation;
    }

    [Inject] private RotationData rotationData;

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int index = 0; index < rotationData.Length; index++)
        {
            var rotation = rotationData.Rotation[index];
            
            rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(settings.playerRotation, settings.playerRotationSpeed * dt));

            rotationData.Rotation[index] = rotation;
            //Comment to try out merge confilct

        }
    }
}
