using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerMoveSystem : ComponentSystem
{


    public struct PlayerMoveData
    {
        public readonly int Length;
        public ComponentDataArray<PlayerPosition> PlayerPositon;
        public ComponentDataArray<Position> PlayerWorldPosition;
    }

    [Inject] private PlayerMoveData playerData;

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < playerData.Length; i++)
        {

            var playerPositon = playerData.PlayerPositon[i];
            var playerWorldPosition = playerData.PlayerWorldPosition[i];

            playerWorldPosition.Value.x = playerPositon.Position * settings.playerSpaceDistance - settings.playerOffset;

            playerData.PlayerWorldPosition[i] = playerWorldPosition;
            
        }
    }
}
