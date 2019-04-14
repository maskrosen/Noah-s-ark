using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ButtonThing : MonoBehaviour
{

    private bool waitingForClick = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            // this creates a horizontal plane passing through this object's center
            var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            // create a ray from the mousePosition
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // plane.Raycast returns the distance from the ray start to the hit point
            float distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                // some point of the plane was hit - get its coordinates
                var hitPoint = ray.GetPoint(distance);
                // use the hitPoint to aim your cannon
                //Debug.Log("hitPoint: " + hitPoint);

                Debug.Log("Generating bunny");
                var entityManager = World.Active.GetOrCreateManager<EntityManager>();
                Entity goal = entityManager.CreateEntity(Bootstrap.GoalArchetype);
                entityManager.AddSharedComponentData(goal, Bootstrap.BunnyLook);
                var goalPosition = new float3(hitPoint.x, 0, hitPoint.z);

                entityManager.SetComponentData(goal, new Scale { Value = new float3(50.0f, 50.0f, 50.0f) });
                entityManager.SetComponentData(goal, new Position { Value = goalPosition });
                entityManager.SetComponentData(goal, new Rotation { Value = quaternion.identity });
                entityManager.SetComponentData(goal, new RadiusComponent { Value = 5 });
            }
            waitingForClick = false;
        }
    }

    public void OnClick()
    {
        waitingForClick = true;
        Debug.Log("Waiting for click");
    }
    
    public void OnClickRestart()
    {
        Bootstrap.ClearGame();
        Bootstrap.NewGame();
        
        Debug.Log("Restarting game");
    }
}
