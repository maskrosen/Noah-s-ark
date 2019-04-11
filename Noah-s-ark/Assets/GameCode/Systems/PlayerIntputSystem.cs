using UnityEngine;
using System.Collections;
using Unity.Entities;

public class PlayerIntputSystem : ComponentSystem
{

    struct PlayerData
    {
        public readonly int Length;

        public ComponentDataArray<PlayerInput> PlayerInput;
        public ComponentDataArray<PlayerFaction> PlayerFaction;
    }

    [Inject] private PlayerData playerData;

    protected override void OnUpdate()
    {
        for (int i = 0; i < playerData.Length; i++)
        {

            if (playerData.PlayerFaction[i].Faction != Factions.Player)
                continue;
            var playerInput = playerData.PlayerInput[i];

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                playerInput.MoveDirection = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                playerInput.MoveDirection = 1;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                playerInput.Shoot = true;
            }

            playerData.PlayerInput[i] = playerInput;

        }
    }

}
