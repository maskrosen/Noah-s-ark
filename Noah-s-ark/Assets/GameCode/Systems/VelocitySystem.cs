using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class VelocitySystem : ComponentSystem
{


    public struct MoveData
    {
        public readonly int Length;
        public ComponentDataArray<VelocityComponent> Velocity;
        public ComponentDataArray<Position> Position;
    }

    [Inject] private MoveData moveData;

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < moveData.Length; i++)
        {

            var velocity = moveData.Velocity[i];
            var position = moveData.Position[i];
            
            position.Value += velocity.Velocity * dt;

            moveData.Position[i] = position;
            
        }
    }
}
