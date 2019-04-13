using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ButtonThing : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
    }

    public void OnClick()
    {
        Debug.Log("Generating bunny");
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        Entity goal = entityManager.CreateEntity(Bootstrap.GoalArchetype);
        entityManager.AddSharedComponentData(goal, Bootstrap.BunnyLook);
        var goalPosition = new float3(15, 0, 5);

        entityManager.SetComponentData(goal, new Scale { Value = new float3(50.0f, 50.0f, 50.0f) });
        entityManager.SetComponentData(goal, new Position { Value = goalPosition });
        entityManager.SetComponentData(goal, new Rotation { Value = quaternion.identity });
        entityManager.SetComponentData(goal, new CircleComponent { Position = goalPosition, Radius = 5 });
    }
}
