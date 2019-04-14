using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class CollisionSystem : ComponentSystem
{
    public struct GoalData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<RadiusComponent> Radius;
        public ComponentDataArray<GoalComponent> GoalComponent;
    }

    public struct BoatData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<RadiusComponent> Radius;
        public ComponentDataArray<BoatComponent> BoatComponent;
    }

    public struct IslandData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<RadiusComponent> Radius;
        public ComponentDataArray<IslandComponent> IslandComponent;
    }

    public Text StatusText;

    [Inject] private GoalData goalData;
    [Inject] private BoatData boatData;
    [Inject] private IslandData islandData;

    public void SetupGameObjects()
    {
        StatusText = GameObject.Find("GameStatusText").GetComponent<Text>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        //For every boat
        for (int i = 0; i < boatData.Length; i++)
        {
            //For every goal
            for (int j = 0; j < goalData.Length; j++)
            {
                if (false && Utils.IsCollidingCircleCircle(boatData.Position[i].Value, boatData.Radius[i].Value, goalData.Position[j].Value, goalData.Radius[j].Value))
                {
                    Debug.Log("Goal reached!!!");
                    StatusText.text = "You got pwnd in the butthole";
                }
            }

            //For every island
            for (int j = 0; j < islandData.Length; j++)
            {
                if (Utils.IsCollidingCircleCircle(boatData.Position[i].Value, boatData.Radius[i].Value, islandData.Position[j].Value, islandData.Radius[j].Value))
                {
                    Debug.Log("LOL YOU DIED!!!");
                    StatusText.text = "LOL YOU DIED!!!";
                }
            }
        }
    }
}
