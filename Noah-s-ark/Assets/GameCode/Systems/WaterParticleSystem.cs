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
        public ComponentDataArray<VelocityComponent> Velocity;
    }

    [Inject] private ParticleData particleData;

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int i = 0; i < particleData.Length; i++)
        {
            var position = particleData.Position[i];
            var velocity = particleData.Velocity[i];
            if(position.Value.x > Constants.HIGH_WORLD_EDGE)
            {
                position.Value.x = Constants.LOW_WORLD_EDGE;
            }

            particleData.Position[i] = position;

            int x = (int)position.Value.x + Constants.WORLD_VECTORFIELD_OFFSET;
            int y = (int)position.Value.z + Constants.WORLD_VECTORFIELD_OFFSET;
            
            int vectorFieldIndex = x * Constants.VECTORFIELD_SIZE + y;

            var vector = VectorField.Get().field[vectorFieldIndex];

            var velocityDirection = new float3(vector.x, 0, vector.y);

            velocity.Value = velocityDirection * Utils.Float3Magnitude(velocity.Value);

            particleData.Velocity[i] = velocity;


        }
    }
}
