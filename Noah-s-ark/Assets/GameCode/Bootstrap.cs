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
    public static EntityArchetype BoatArchetype;
    public static EntityArchetype VectorFieldArchetype;

    public static RenderMesh PlayerLook;
    public static RenderMesh BoatLook;

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

        VectorFieldArchetype = entityManager.CreateArchetype(typeof(VectorField));
        
        BoatArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(TurnRateComponent), typeof(Scale), typeof(BoatComponent));

    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        Settings = settingsGO?.GetComponent<Settings>();
        if (!Settings)
            return;

        PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
        BoatLook = GetLookFromPrototype("BoatRenderPrototype");

        World.Active.GetOrCreateManager<GameOverSystem>().SetupGameObjects();
        NewGame();
    }


    public static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity gameState = entityManager.CreateEntity(GameStateArchetype);

        Entity VectorField = entityManager.CreateEntity(VectorFieldArchetype);
        entityManager.SetComponentData(VectorField, new VectorField {Value = new Vector2[Constants.VECTORFIELD_SIZE* Constants.VECTORFIELD_SIZE] });

        Entity boat = entityManager.CreateEntity(BoatArchetype);
        entityManager.AddSharedComponentData(boat, BoatLook);
        entityManager.SetComponentData(boat, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
        entityManager.SetComponentData(boat, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.SetComponentData(boat, new Rotation { Value = /*quaternion.Euler(-90f, 0, 0)*/   quaternion.identity });

        entityManager.SetComponentData(boat, new TurnRateComponent { TurnRate = 90 });
        entityManager.SetComponentData(boat, new VelocityComponent { Velocity =  new float3(0, 0, 8)});
    
        /*


        var turnState = new GameState { CurrentTurnFaction = UnityEngine.Random.Range(0, 2), CurrentState = GameStates.Playing};

        entityManager.SetComponentData(gameState, turnState);

        Entity player = entityManager.CreateEntity(PlayerArchetype);

        entityManager.SetComponentData(player, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        var rotation = new Rotation { Value = /*quaternion.Euler(-1.5f,0,0)  quaternion.identity };
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

        entityManager.AddSharedComponentData(enemy, PlayerLook); */


    }


    private static RenderMesh GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<RenderMeshProxy>().Value;
        Object.Destroy(proto);
        return result;
    }

}
