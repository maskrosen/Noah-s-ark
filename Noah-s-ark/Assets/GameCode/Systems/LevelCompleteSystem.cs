using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteSystem : ComponentSystem
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

    public Text StatusText;

    [Inject] private GoalData goalData;
    [Inject] private BoatData boatData;

    public void SetupGameObjects()
    {
        StatusText = GameObject.Find("GameStatusText").GetComponent<Text>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < goalData.Length; i++)
        {
            for (int j = 0; j < boatData.Length; j++)
            {
                var boatPos = boatData.Position[j].Value;
                var goalCircle = goalData.Circle[i];
                var diffVector = boatPos - goalCircle.Position;
                var distanceSq = Utils.Float3MagnitudeSq(diffVector);
                if (distanceSq < goalCircle.Radius*goalCircle.Radius)
                {
                    Debug.Log("Goal reached!!!");
                    StatusText.text = "You got pwnd in the butt";
                }
            }
        }
    }
}
