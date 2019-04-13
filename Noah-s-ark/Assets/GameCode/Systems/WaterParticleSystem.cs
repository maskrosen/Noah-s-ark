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

    private Unity.Mathematics.Random random = new Unity.Mathematics.Random(835483957);

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int i = 0; i < particleData.Length; i++)
        {
            var position = particleData.Position[i];
            var velocity = particleData.Velocity[i];
            var particleComponent = particleData.Particle[i];

            bool randomizePosition = false;
            if(position.Value.x > Constants.HIGH_WORLD_EDGE)
            {
                randomizePosition = true;
            }

            if (position.Value.x < Constants.LOW_WORLD_EDGE)
            {
                randomizePosition = true;
            }

            if (position.Value.z > Constants.HIGH_WORLD_EDGE)
            {
                randomizePosition = true;
            }

            if (position.Value.z < Constants.LOW_WORLD_EDGE)
            {
                randomizePosition = true;
            }

            particleComponent.LifeTimeLeft -= dt;

            if (particleComponent.LifeTimeLeft < 0)
                randomizePosition = true;

            if (randomizePosition)
            {
                position.Value = random.NextFloat3() * 100 - Constants.HIGH_WORLD_EDGE;
                position.Value.y = 0;
                particleComponent.LifeTimeLeft = Constants.PARTICLE_LIFETIME;
            }


            int x = (int)position.Value.x + Constants.WORLD_VECTORFIELD_OFFSET;
            int y = (int)position.Value.z + Constants.WORLD_VECTORFIELD_OFFSET;

            x = math.min(x, Constants.VECTORFIELD_SIZE - 1);
            x = math.max(x, 0);

            y = math.min(x, Constants.VECTORFIELD_SIZE - 1);
            y = math.max(x, 0);

            int vectorFieldIndex = x * Constants.VECTORFIELD_SIZE + y;

            var vector = VectorField.Get().field[vectorFieldIndex];

            var velocityDirection = new float3(vector.x, 0, vector.y);

            velocity.Value = velocityDirection;

            particleData.Velocity[i] = velocity;
            particleData.Particle[i] = particleComponent;
            particleData.Position[i] = position;


        }
    }
}
