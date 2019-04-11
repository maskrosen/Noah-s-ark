using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


[UpdateBefore(typeof(GameOverSystem))]
public class HitOrMissSystem : ComponentSystem
{


    public struct BulletData
    {
        public readonly int Length;
        public ComponentDataArray<BulletComponent> Bullet;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<PlayerFaction> Faction;
        public ComponentDataArray<PlayerPosition> PlayerPosition;
    }

    public struct PlayerData
    {
        public readonly int Length;
        public ComponentDataArray<PlayerComponent> Player;
        public ComponentDataArray<PlayerFaction> Faction;
        public ComponentDataArray<PlayerPosition> PlayerPosition;
    }

    public struct GameStateData
    {
        public readonly int Length;
        public ComponentDataArray<GameState> GameState;
    }

    [Inject] private BulletData bulletData;
    [Inject] private PlayerData playerData;
    [Inject] private GameStateData gameStateData;
    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        if (gameStateData.GameState[0].CurrentState != GameStates.Playing)
            return;

        for (int i = 0; i < bulletData.Length; i++)
        {

            var faction = bulletData.Faction[i];
            var position = bulletData.PlayerPosition[i];

            for(int j = 0; j < playerData.Length; j++)
            {
                var playerFaction = playerData.Faction[j];
                var playerPosition = playerData.PlayerPosition[j];

                if (faction.Faction != playerFaction.Faction && position.Position == playerPosition.Position)
                {
                    var entity = PostUpdateCommands.CreateEntity(Bootstrap.GameOverArchetype);
                    PostUpdateCommands.SetComponent(entity, new GameOverComponent { WinningFaction = faction.Faction});
                }
            }
            
        }
    }
}
