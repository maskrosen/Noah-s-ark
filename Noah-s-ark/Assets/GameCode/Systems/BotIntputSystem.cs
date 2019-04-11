using UnityEngine;
using System.Collections;
using Unity.Entities;

public class BotIntputSystem : ComponentSystem
{

    struct PlayerData
    {
        public readonly int Length;

        public ComponentDataArray<PlayerInput> PlayerInput;
        public ComponentDataArray<PlayerFaction> PlayerFaction;
        public ComponentDataArray<PlayerTurnState> PlayerTurnState;
        public ComponentDataArray<BotState> BotState;
    }

    [Inject] private PlayerData playerData;

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < playerData.Length; i++)
        {


            var botState = playerData.BotState[i];               

            if (!playerData.PlayerTurnState[i].PlayersTurn)
            {
                botState.TurnCooldown = settings.botTurnCooldown;
                playerData.BotState[i] = botState;
                continue;
            }

            botState.TurnCooldown -= dt;

            if (botState.TurnCooldown > 0)
            {

                playerData.BotState[i] = botState;
                continue;
            }

            var playerInput = playerData.PlayerInput[i];

            var move = Random.Range(-1, 2);
            playerInput.MoveDirection = move;
            if (move == 0)
                playerInput.Shoot = true;
            
            playerData.PlayerInput[i] = playerInput;

        }
    }

}
