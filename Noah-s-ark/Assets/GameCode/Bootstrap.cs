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
    public static EntityArchetype WaterParticleArchetype;
    public static EntityArchetype GoalArchetype;

    public static RenderMesh FoxLook;
    public static RenderMesh BunnyLook;
    public static RenderMesh BoatLook;
    public static RenderMesh WaterParticleLook;

    public static Settings Settings;

    private VectorField vectorField;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        VectorField.Initialize();

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        PlayerArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(PlayerPosition), typeof(PlayerInput), typeof(PlayerTurnState), typeof(PlayerFaction), typeof(PlayerComponent));

        BotArchetype = entityManager.CreateArchetype(
            typeof(Position), typeof(Rotation), typeof(PlayerPosition), typeof(PlayerInput), typeof(PlayerTurnState), typeof(PlayerFaction), typeof(BotState), typeof(PlayerComponent));

        GameStateArchetype = entityManager.CreateArchetype(typeof(GameState));

        BulletArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(PlayerPosition), typeof(PlayerFaction), typeof(TimerComponent), typeof(BulletComponent));

        GameOverArchetype = entityManager.CreateArchetype(typeof(GameOverComponent));
        
        BoatArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(TurnRateComponent), typeof(Scale), typeof(BoatComponent));

        BoatArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(TurnRateComponent), typeof(Scale));

        WaterParticleArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Scale), typeof(VelocityComponent), typeof(ParticleComponent));

        GoalArchetype = entityManager.CreateArchetype(typeof(CircleComponent), typeof(Position), typeof(Rotation), typeof(Scale), typeof(GoalComponent));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        Settings = settingsGO?.GetComponent<Settings>();
        if (!Settings)
            return;

        FoxLook = GetLookFromPrototype("FoxRenderPrototype");
        BunnyLook = GetLookFromPrototype("BunnyRenderPrototype");
        BoatLook = GetLookFromPrototype("BoatRenderPrototype");
        WaterParticleLook = GetLookFromPrototype("WaterParticleRenderPrototype");

        World.Active.GetOrCreateManager<LevelCompleteSystem>().SetupGameObjects();
        NewGame();
    }

    public static void SpawnParticles()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var random = new Unity.Mathematics.Random(835483957);
        for (int i = 0; i < 5000; i++)
        {            
            Entity particle = entityManager.CreateEntity(WaterParticleArchetype);
            entityManager.AddSharedComponentData(particle, WaterParticleLook);
            var position = random.NextFloat3() * 100 - Constants.HIGH_WORLD_EDGE;
            position.y = 0;
            var velocity = random.NextFloat3() * 4f - 2;
            velocity.y = 0;
            entityManager.SetComponentData(particle, new Scale { Value = new float3(0.2f)});
            entityManager.SetComponentData(particle, new Position { Value = position });
            entityManager.SetComponentData(particle, new Rotation { Value = quaternion.identity });
            entityManager.SetComponentData(particle, new VelocityComponent { Value = new float3(1, 0, 0) });
        }
    }

    public static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity gameState = entityManager.CreateEntity(GameStateArchetype);


        Entity boat = entityManager.CreateEntity(BoatArchetype);
        entityManager.AddSharedComponentData(boat, BoatLook);
        entityManager.SetComponentData(boat, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
        entityManager.SetComponentData(boat, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.SetComponentData(boat, new Rotation { Value = /*quaternion.Euler(-90f, 0, 0)*/ quaternion.identity });
        entityManager.SetComponentData(boat, new TurnRateComponent { TurnRate = 10 });
        entityManager.SetComponentData(boat, new VelocityComponent { Value = new float3(0, 0, 8)});

        SpawnParticles();
    

        /*
          //When we have multiple levels, the bunny can be used as well. scale with 100
          entityManager.AddSharedComponentData(goal, BunnyLook);
          entityManager.SetComponentData(goal, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
         */
        var goalPosition = new float3(5, 0, 15);
        Entity goal = entityManager.CreateEntity(GoalArchetype);
        entityManager.AddSharedComponentData(goal, FoxLook);
        entityManager.SetComponentData(goal, new Scale { Value = new float3(2000.0f, 2000.0f, 2000.0f) });
        entityManager.SetComponentData(goal, new Position { Value = goalPosition });
        entityManager.SetComponentData(goal, new Rotation { Value = /*quaternion.Euler(-90f, 0, 0)*/ quaternion.identity });
        entityManager.SetComponentData(goal, new CircleComponent { Position = goalPosition, Radius = 5 });

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

        entityManager.AddSharedComponentData(enemy, PlayerLook);
        */
    }

    private static RenderMesh GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<RenderMeshProxy>().Value;
        Object.Destroy(proto);
        return result;
    }
}
