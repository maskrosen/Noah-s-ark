using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public sealed class Bootstrap
{


    public static EntityArchetype PlayerArchetype;
    public static EntityArchetype BotArchetype;
    public static EntityArchetype GameStateArchetype;
    public static EntityArchetype BulletArchetype;
    public static EntityArchetype GameOverArchetype;

    public static RenderMesh PlayerLook;
    public static RenderMesh BulletLook;

    public static Settings Settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        PlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(PlayerPosition), typeof(PlayerInput), typeof(PlayerTurnState), typeof(PlayerFaction), typeof(PlayerComponent));

        BotArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(PlayerPosition), typeof(PlayerInput), typeof(PlayerTurnState), typeof(PlayerFaction), typeof(BotState), typeof(PlayerComponent));

        GameStateArchetype = entityManager.CreateArchetype(typeof(GameState));

        BulletArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(PlayerPosition), typeof(PlayerFaction), typeof(TimerComponent), typeof(BulletComponent));

        GameOverArchetype = entityManager.CreateArchetype(typeof(GameOverComponent));

    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        Settings = settingsGO?.GetComponent<Settings>();
        if (!Settings)
            return;

        PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
        BulletLook = GetLookFromPrototype("BulletRenderPrototype");

        World.Active.GetOrCreateManager<GameOverSystem>().SetupGameObjects();

        NewGame();
    }


    public static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();              

        Entity gameState = entityManager.CreateEntity(GameStateArchetype);

        var turnState = new GameState { CurrentTurnFaction = UnityEngine.Random.Range(0, 2), CurrentState = GameStates.Playing};

        entityManager.SetComponentData(gameState, turnState);

        Entity player = entityManager.CreateEntity(PlayerArchetype);

        entityManager.SetComponentData(player, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        var rotation = new Rotation { Value = /*quaternion.Euler(-1.5f,0,0) */ quaternion.identity };
        rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(new float3(1,0,0), -(float)(math.PI/2)));
        entityManager.SetComponentData(player, rotation);
        var faction = new PlayerFaction { Faction = Factions.Player};
        entityManager.SetComponentData(player, faction);
        entityManager.SetComponentData(player, new PlayerPosition { Position = UnityEngine.Random.Range(0, Settings.playerStandingSpots) });

        entityManager.AddSharedComponentData(player, PlayerLook);

        // Add enemy player
        Entity enemy = entityManager.CreateEntity(BotArchetype);

        entityManager.SetComponentData(enemy, new Position { Value = new float3(0.0f, 0.0f, Settings.distanceBetweenPlayers) });
        entityManager.SetComponentData(enemy, rotation);
        var factionEnemy = new PlayerFaction { Faction = Factions.Enemy };
        entityManager.SetComponentData(enemy, factionEnemy);
        entityManager.SetComponentData(enemy, new PlayerPosition { Position = UnityEngine.Random.Range(0, Settings.playerStandingSpots) });
        var botState = new BotState { TurnCooldown = Settings.botTurnCooldown };

        entityManager.AddSharedComponentData(enemy, PlayerLook);


    }


    private static RenderMesh GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<RenderMeshProxy>().Value;
        Object.Destroy(proto);
        return result;
    }

}
