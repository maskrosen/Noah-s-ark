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

    public struct MeteoriteData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<RadiusComponent> Radius;
        public ComponentDataArray<MeteoriteComponent> MeteoriteComponent;
    }

    public struct GameStateData
    {
        public readonly int Length;
        public ComponentDataArray<GameStateComponent> gamestate;
    }

    public Text StatusText;

    [Inject] private GoalData goalData;
    [Inject] private BoatData boatData;
    [Inject] private IslandData islandData;
    [Inject] private MeteoriteData meteoriteData;
    [Inject] private GameStateData gameStateData;

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
                if (Utils.IsCollidingCircleCircle(boatData.Position[i].Value, boatData.Radius[i].Value, goalData.Position[j].Value, goalData.Radius[j].Value))
                {
                    /*
                    Time.timeScale = 0;
                    StatusText.text = "You got pwnd in the butthole";
                    */
                    for (int k = 0; k < gameStateData.Length; k++) {
                        Bootstrap.ClearGame();
                        Bootstrap.NewGame(gameStateData.gamestate[k].currentLevel + 1);
                    }
                }
            }

            //For every island
            for (int j = 0; j < islandData.Length; j++)
            {
                if (Utils.IsCollidingCircleCircle(boatData.Position[i].Value, boatData.Radius[i].Value, islandData.Position[j].Value, islandData.Radius[j].Value))
                {
                    Time.timeScale = 0;
                    StatusText.text = "LOL YOU DIED!!!";
                }
            }

            //For every meteorite
            for (int k = 0; k < meteoriteData.Length; k++)
            {
                //TODO: Do stuff
                if (Utils.IsCollidingCircleSphere(boatData.Position[i].Value, boatData.Radius[i].Value, meteoriteData.Position[k].Value, meteoriteData.Radius[k].Value))
                {
                    Time.timeScale = 0;
                    StatusText.text = "LOL YOU GOT BY A MeTeOrItE!!!";
                }
            }
        }
    }
}
