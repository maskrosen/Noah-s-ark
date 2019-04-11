using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class GameOverSystem : ComponentSystem
{


    public struct GameOverData
    {
        public readonly int Length;
        public ComponentDataArray<GameOverComponent> GameOver;
        public EntityArray Entities;
    }

    public struct PlayerTurnData
    {
        public readonly int Length;
        public ComponentDataArray<PlayerTurnState> TurnState;
    }

    public struct GameStateData
    {
        public readonly int Length;
        public ComponentDataArray<GameState> GameState;
    }

    public Text StatusText;

    [Inject] private GameOverData gameOverData;
    [Inject] private PlayerTurnData playerTurnData;
    [Inject] private GameStateData gameStateData;


    public void SetupGameObjects()
    {
        StatusText = GameObject.Find("GameStatusText").GetComponent<Text>();
    }


    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < gameOverData.Length; i++)
        {

            var gameOver = gameOverData.GameOver[i];
            if(gameOver.WinningFaction == Factions.Player)
            {
                StatusText.text = "You won!!!";
            }
            else
            {
                StatusText.text = "You got pwnd in the butt";
            }

            PostUpdateCommands.DestroyEntity(gameOverData.Entities[i]);

            for(int j = 0; j < playerTurnData.Length; j++)
            {
                var playerTurnState = playerTurnData.TurnState[j];
                playerTurnState.PlayersTurn = false;
                playerTurnState.TurnDone = false;

                playerTurnData.TurnState[j] = playerTurnState;
            }

            var gameState = gameStateData.GameState[0];
            gameState.CurrentTurnFaction = Factions.None;
            gameState.CurrentState = GameStates.GameOver;

            gameStateData.GameState[0] = gameState;
            
        }
    }
}
