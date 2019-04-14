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

    public struct WindData
    {
        public ComponentDataArray<WindComponent> WindComponent;
        public ComponentDataArray<TimerComponent> TimerComponent;
    }

    [Inject] private BoatData boatData;
    [Inject] private WindData windData;

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
           
            Vector2 vel = VectorField.Get().VectorAtPos(position.Value);
            var newVelocity = new float3(vel.x, 0, vel.y);

            if(windData.WindComponent.Length > 0)
            {
                var wind = windData.WindComponent[0]; //Only one wind active at the same time
                var timer = windData.TimerComponent[0];
                Vector2 lerpedVector;
                var vector = wind.Velocity.normalized * Constants.WIND_POWERUP_SPEED;
                if (timer.CurrentTime / timer.Duration < 0.5)
                {
                    lerpedVector = Vector2.Lerp(new Vector2(0, 0), vector, timer.CurrentTime / timer.Duration);

                }
                else
                {
                    lerpedVector = Vector2.Lerp(vector, new Vector2(0, 0), timer.CurrentTime / timer.Duration);
                }
                var windVelocity = new float3(lerpedVector.x, 0, lerpedVector.y);


                newVelocity += windVelocity;
            }

            boatData.Velocity[i] = new VelocityComponent { Value = newVelocity };

            if (Utils.Float3Magnitude(newVelocity) < 0.001f)
            {
                //dont change rotation
            }
            else
            {
                boatData.Rotation[i] = new Rotation { Value = Quaternion.LookRotation(newVelocity, Vector3.up) };
            }

            
        }
    }
}
