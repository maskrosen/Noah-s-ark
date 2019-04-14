using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WindSystem : ComponentSystem
{


    public struct WindData
    {
        public readonly int Length;
        public ComponentDataArray<WindComponent> Velocity;
    }

    public struct DragGestureData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<DragGestureComponent> DragGesture;
    }

    [Inject]
    private WindData windData;

    [Inject]
    private DragGestureData dragGestureData;

    private EntityArchetype WindArchetype;

    public void Init()
    {
        WindArchetype = EntityManager.CreateArchetype(typeof(WindComponent), typeof(TimerComponent));
    }

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for(int i = 0; i < dragGestureData.Length; i++)
        {
            var pos1 = dragGestureData.DragGesture[i].ClickPosition;
            var pos2 = dragGestureData.DragGesture[i].ReleasePosition;

            Vector3 fixedPos1 = new Vector3();
            Vector3 fixedPos2 = new Vector3();

            // this creates a horizontal plane passing through this object's center
            var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            // create a ray from the mousePosition
            var ray = Camera.main.ScreenPointToRay(pos1);
            // plane.Raycast returns the distance from the ray start to the hit point
            float distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                // some point of the plane was hit - get its coordinates
                var hitPoint = ray.GetPoint(distance);
                // use the hitPoint to aim your cannon
                fixedPos1 = new float3(hitPoint.x, 0, hitPoint.z);
            }
            
            // create a ray from the mousePosition
            ray = Camera.main.ScreenPointToRay(pos2);
            // plane.Raycast returns the distance from the ray start to the hit point
            distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                // some point of the plane was hit - get its coordinates
                var hitPoint = ray.GetPoint(distance);
                // use the hitPoint to aim your cannon
                fixedPos2 = new float3(hitPoint.x, 0, hitPoint.z);
            }

            var diff = fixedPos2 - fixedPos1;

            var wind = PostUpdateCommands.CreateEntity(WindArchetype);
            PostUpdateCommands.SetComponent(wind, new WindComponent { Velocity = new Vector2(diff.x, diff.z)});
            PostUpdateCommands.SetComponent(wind, new TimerComponent { Duration = Constants.WIND_POWERUP_TIME, DeleteOnEnd = true });

        }

        for (int i = 0; i < windData.Length; i++)
        {

            
            
        }
    }
}
