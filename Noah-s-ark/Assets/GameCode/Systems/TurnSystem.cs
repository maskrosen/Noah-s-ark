using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class TurnSystem : ComponentSystem
{
    
    public struct PlayerTurnData
    {
        public readonly int Length;
        public ComponentDataArray<PlayerTurnState> PlayerTurn;
        public ComponentDataArray<PlayerFaction> PlayerFaction;
    }

    public struct GameStateData
    {
        public readonly int Length;
        public ComponentDataArray<GameState> TurnState;
    }

    [Inject] private PlayerTurnData playerData;
    [Inject] private GameStateData gameStateData;

    protected override void OnUpdate()
    {

        var settings = Bootstrap.Settings;

        if (gameStateData.Length > 1)
            throw new System.InvalidOperationException("Only one game state is allowed");

        var turnState = gameStateData.TurnState[0];

        for (int i = 0; i < playerData.Length; i++)
        {
            var playerTurnState = playerData.PlayerTurn[i];
            var playerFaction = playerData.PlayerFaction[i];

            if (playerTurnState.TurnDone)
                turnState.CurrentTurnFaction = Factions.OtherPlayer(playerFaction.Faction);

            playerTurnState.TurnDone = false;


            playerData.PlayerTurn[i] = playerTurnState;
        }

        for (int i = 0; i < playerData.Length; i++)
        {
            var playerTurnState = playerData.PlayerTurn[i];
            var playerFaction = playerData.PlayerFaction[i];

            playerTurnState.PlayersTurn = turnState.CurrentTurnFaction == playerFaction.Faction;
            playerData.PlayerTurn[i] = playerTurnState;
        }

        gameStateData.TurnState[0] = turnState;

    }
}
