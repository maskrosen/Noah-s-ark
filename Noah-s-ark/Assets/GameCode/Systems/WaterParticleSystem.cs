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

            if (position.Value.x < Constants.LOW_WORLD_EDGE)
            {
                position.Value.x = Constants.HIGH_WORLD_EDGE;
            }

            if (position.Value.z > Constants.HIGH_WORLD_EDGE)
            {
                position.Value.z = Constants.LOW_WORLD_EDGE;
            }

            if (position.Value.z < Constants.LOW_WORLD_EDGE)
            {
                position.Value.z = Constants.HIGH_WORLD_EDGE;
            }

            particleData.Position[i] = position;

            /*
            int x = (int)position.Value.x + Constants.WORLD_VECTORFIELD_OFFSET;
            int y = (int)position.Value.z + Constants.WORLD_VECTORFIELD_OFFSET;

            x = math.min(x, Constants.VECTORFIELD_SIZE - 1);
            x = math.max(x, 0);

            y = math.min(x, Constants.VECTORFIELD_SIZE - 1);
            y = math.max(x, 0);

            int vectorFieldIndex = x * Constants.VECTORFIELD_SIZE + y;

            var vector = VectorField.Get().field[vectorFieldIndex];
            
            var velocityDirection = new float3(vector.x, 0, vector.y);
            
            particleData.Velocity[i] = velocity;
            */
            Vector2 vel = VectorField.Get().VectorAtPos(position.Value);
            var newVelocity = new float3(vel.x, 0, vel.y);
            particleData.Velocity[i] = new VelocityComponent { Value = newVelocity };
        }
    }
}
