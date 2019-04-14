using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WaveSystem : ComponentSystem
{


    public struct WaveData
    {
        public readonly int Length;
        public ComponentDataArray<WaveComponent> Wave;
        public ComponentDataArray<Position> Position;
    }

    [Inject] private WaveData waveData;

    protected override void OnUpdate()
    {
        const float thickness = 3f;
        const float radiusExpansionPerSecond = 5f;
        float dt = Time.deltaTime;

        VectorField vf = VectorField.Get();

        for (int i = 0; i < waveData.Length; i++)
        {
            float3 pos = waveData.Position[i].Value;
            float innerRadius = waveData.Wave[i].Radius;
            float outerRadius = innerRadius + thickness;
            float lifeLeft = waveData.Wave[i].LifeTimeLeft;
            bool justSpawned = waveData.Wave[i].justSpawned;

            //if not just spawned, undo previous wave
            if(!justSpawned)
                vf.AddPushingRing(pos, -lifeLeft, outerRadius, innerRadius);

            //update values
            lifeLeft -= dt;
            waveData.Wave[i] = new WaveComponent
            {
                justSpawned = false,
                LifeTimeLeft = lifeLeft - dt,
                Radius = innerRadius += radiusExpansionPerSecond
            };
            //

            //spawn new wave
            innerRadius = waveData.Wave[i].Radius;
            outerRadius = innerRadius + thickness;
            lifeLeft = waveData.Wave[i].LifeTimeLeft;
            
            if(lifeLeft <= 0)
            {
                //TODO kill somehow?
            }

            vf.AddPushingRing(pos, -lifeLeft, outerRadius, innerRadius);
        }
    }
}