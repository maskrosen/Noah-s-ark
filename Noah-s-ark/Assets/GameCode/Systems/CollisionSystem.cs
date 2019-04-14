using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class CollisionSystem : ComponentSystem
{
    public struct GoalData
    {
        public readonly int Length;
        public ComponentDataArray<CircleComponent> Circle;
        public ComponentDataArray<GoalComponent> GoalComponent;
    }

    public struct BoatData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<BoatComponent> BoatComponent;
    }

    public struct IslandData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<BoatComponent> IslandComponent;
    }

    public Text StatusText;

    [Inject] private GoalData goalData;
    [Inject] private BoatData boatData;
    [Inject] private BoatData islandData;

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
                
                var boatPos = boatData.Position[i].Value;
                var goalCircle = goalData.Circle[j];
                var diffVector = boatPos - goalCircle.Position;
                var distanceSq = Utils.Float3MagnitudeSq(diffVector);
                if (distanceSq < goalCircle.Radius*goalCircle.Radius)
                {
                    Debug.Log("Goal reached!!!");
                    StatusText.text = "You got pwnd in the butt";
                }
            }

            //For every island
            for (int j = 0; j < islandData.Length; j++)
            {
                //TODO: Do stuff
            }
        }
    }
}
