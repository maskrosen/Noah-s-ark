using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class PlayerControlSystem : ComponentSystem
{


    public struct PlayerData
    {
        public readonly int Length;
        public ComponentDataArray<PlayerPosition> PlayerPositon;
        public ComponentDataArray<PlayerInput> PlayerInput;
        public ComponentDataArray<PlayerTurnState> PlayerTurnState;
        public ComponentDataArray<PlayerFaction> PlayerFaction;
        public ComponentDataArray<Position> Position;
    }

    [Inject] private PlayerData playerData;

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        for (int i = 0; i < playerData.Length; i++)
        {

            var playerInput = playerData.PlayerInput[i];
            var playerPositon = playerData.PlayerPositon[i];
            var playerTurnState = playerData.PlayerTurnState[i];
            var playerFaction = playerData.PlayerFaction[i];
            var position = playerData.Position[i];
            if(playerInput.MoveDirection != 0 && playerTurnState.PlayersTurn)
            {
                var playerMoved = true;
                playerPositon.Position += playerInput.MoveDirection;

                if (playerPositon.Position >= settings.playerStandingSpots)
                {
                    playerPositon.Position = settings.playerStandingSpots - 1;
                    playerMoved = false;
                }
                else if(playerPositon.Position < 0)
                {
                    playerPositon.Position = 0;
                    playerMoved = false;
                }


                if (playerMoved)
                {
                    playerTurnState.TurnDone = true;
                }

            }

            if(playerInput.Shoot && playerTurnState.PlayersTurn)
            {
                var entity = PostUpdateCommands.CreateEntity(Bootstrap.BulletArchetype);
                PostUpdateCommands.SetComponent(entity, position);

                var rotation = new Rotation { Value = quaternion.identity };
                rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(new float3(1, 0, 0), -(float)(math.PI / 2)));

                PostUpdateCommands.SetComponent(entity, rotation);
                PostUpdateCommands.SetComponent(entity, new VelocityComponent { Velocity =  new float3(0, 0, playerFaction.Faction == Factions.Enemy ? -settings.bulletSpeed: settings.bulletSpeed)});
                PostUpdateCommands.SetComponent(entity, playerPositon);
                PostUpdateCommands.SetComponent(entity, playerFaction);
                PostUpdateCommands.SetComponent(entity, new TimerComponent { Duration = settings.bulletLifeDuration, CurrentTime = settings.bulletLifeDuration, DeleteOnEnd = true });
                PostUpdateCommands.AddSharedComponent(entity, Bootstrap.BulletLook);

                playerTurnState.TurnDone = true;


            }

            playerInput.MoveDirection = 0;
            playerInput.Shoot = false;
            playerData.PlayerTurnState[i] = playerTurnState;
            playerData.PlayerPositon[i] = playerPositon;
            playerData.PlayerInput[i] = playerInput;
        }
    }
}
