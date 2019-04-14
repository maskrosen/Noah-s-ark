using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class BoatControlSystem : ComponentSystem
{

    public struct BoatData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Rotation;
        public ComponentDataArray<VelocityComponent> Velocity;
        public ComponentDataArray<TurnRateComponent> TurnRate;
        public ComponentDataArray<BoatComponent> BoatComponent;
    }

    [Inject] private BoatData boatData;

    protected override void OnUpdate()
    {
        var settings = Bootstrap.Settings;

        float dt = Time.deltaTime;
        for (int i = 0; i < boatData.Length; i++)
        {
            var velocity = boatData.Velocity[i];
            var turnRate = boatData.TurnRate[i];
            var rotation = boatData.Rotation[i];
            var position = boatData.Position[i];

            /*
            var newRot = Quaternion.Euler(0, turnRate.TurnRate*dt, 0) * rotation.Value;
            newRot.Normalize();
            boatData.Rotation[i] = new Rotation { Value = newRot };
            
            var velocityDirection = newRot * Vector3.forward;

            //

            var velocityVector = new Vector3(velocity.Value.x, velocity.Value.y, velocity.Value.z);
            
            velocityDirection *= velocityVector.magnitude;

            var newVelocity = new float3(velocityDirection.x, velocityDirection.y, velocityDirection.z);
            boatData.Velocity[i] = new VelocityComponent { Velocity = newVelocity };
            */
            
            Vector2 vel = VectorField.Get().VectorAtPos(position.Value);
            var newVelocity = new float3(vel.x, 0, vel.y);
            boatData.Velocity[i] = new VelocityComponent { Value = newVelocity };

            boatData.Rotation[i] = new Rotation { Value = Quaternion.LookRotation(newVelocity, Vector3.up) };
        }
    }
}
