using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MeteoriteSystem : ComponentSystem
{

    public struct MeteoriteData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<VelocityComponent> Velocity;
        public ComponentDataArray<Rotation> Rotation;
        public ComponentDataArray<RotationVelocity> RotationVelocity;
        public ComponentDataArray<MeteoriteComponent> Meteorite;

    }

    [Inject] private MeteoriteData meteoriteData;

    private Unity.Mathematics.Random random = new Unity.Mathematics.Random(835483957);

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int i = 0; i < meteoriteData.Length; i++)
        {
            if (meteoriteData.Position[i].Value.y < -20)
            {
                meteoriteData.Position[i] = new Position { Value = random.NextFloat3() * 20 + new float3(-10f, 50f, -10f) };
                meteoriteData.Velocity[i] = new VelocityComponent { Value = random.NextFloat3() * 6 + new float3(-3f, -10f, -3f) };
                
            }

            float3 rotationVelocity = meteoriteData.RotationVelocity[i].Value;
            quaternion newRot = meteoriteData.Rotation[i].Value * Quaternion.Euler(rotationVelocity.x * dt, rotationVelocity.y * dt, rotationVelocity.z * dt);
            //Vector3 v = Quaternion.ToEulerAngles(meteoriteData.Rotation[i].Value);
            //v.x = v.x + rotationVelocity.x * dt;
            //v.y = v.y + rotationVelocity.y * dt;
            //v.z = v.z + rotationVelocity.z * dt;

            //meteoriteData.Rotation[i] = new Rotation { Value = Quaternion.Euler(v) };
            meteoriteData.Rotation[i] = new Rotation { Value = newRot };

        }

    }

    private quaternion GetValue(int i)
    {
        return meteoriteData.Rotation[i].Value;
    }
}
