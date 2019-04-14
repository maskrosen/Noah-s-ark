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
            if(meteoriteData.Position[i].Value.y < -20)
            {
                meteoriteData.Position[i] = new Position { Value = random.NextFloat3() * 20 + new float3(-10f, 50f, -10f) };
                meteoriteData.Velocity[i] = new VelocityComponent { Value = random.NextFloat3() * 6 + new float3(-3f, -10f, -3f) };
            }

        }
    }
}
