using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TimerSystem : ComponentSystem
{


    public struct TimerData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<TimerComponent> Timer;
    }

    [Inject] private TimerData timerData;

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < timerData.Length; i++)
        {

            var timer = timerData.Timer[i];

            timer.CurrentTime -= dt;

            if (timer.CurrentTime <= 0)
            {
                if (timer.DeleteOnEnd)
                    PostUpdateCommands.DestroyEntity(timerData.Entities[i]);
                else
                    PostUpdateCommands.RemoveComponent<TimerComponent>(timerData.Entities[i]);

            }
            timerData.Timer[i] = timer;

        }
    }
}
