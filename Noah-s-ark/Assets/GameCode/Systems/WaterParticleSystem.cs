using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class WaterParticleSystem : ComponentSystem
{

    public struct ParticleData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<ParticleComponent> Particle;
    }

    [Inject] private ParticleData particleData;

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int i = 0; i < particleData.Length; i++)
        {
            var position = particleData.Position[i];
            if(position.Value.x > 100)
            {
                position.Value.x = -100;
            }

            particleData.Position[i] = position;

        }
    }
}
