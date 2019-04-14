using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public sealed class Bootstrap
{
    public static EntityArchetype BoatArchetype;
    public static EntityArchetype WaterParticleArchetype;
    public static EntityArchetype GoalArchetype;
    public static EntityArchetype IslandArchetype;
    public static EntityArchetype MeteoriteArchetype;

    public static RenderMesh FoxLook;
    public static RenderMesh BunnyLook;
    public static RenderMesh BoatLook;
    public static RenderMesh WaterParticleLook;
    public static RenderMesh IslandLook;
    public static RenderMesh MeteoriteLook;

    public static Settings Settings;

    private VectorField vectorField;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        VectorField.Initialize();

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        
        BoatArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(TurnRateComponent), typeof(Scale), typeof(BoatComponent));
        WaterParticleArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Scale), typeof(VelocityComponent), typeof(ParticleComponent));
        GoalArchetype = entityManager.CreateArchetype(typeof(CircleComponent), typeof(Position), typeof(Rotation), typeof(Scale), typeof(GoalComponent));
        IslandArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Scale), typeof(IslandComponent));
        MeteoriteArchetype = entityManager.CreateArchetype(typeof(Position), typeof(VelocityComponent), typeof(Scale), typeof(MeteoriteComponent), typeof(Rotation), typeof(RotationVelocity));
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
        IslandLook = GetLookFromPrototype("IslandRenderPrototype");
        MeteoriteLook = GetLookFromPrototype("MeteoriteRenderPrototype");

        World.Active.GetOrCreateManager<CollisionSystem>().SetupGameObjects();
        NewGame();
    }


    public static void SpawnBoat()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity boat = entityManager.CreateEntity(BoatArchetype);
        entityManager.AddSharedComponentData(boat, BoatLook);
        entityManager.SetComponentData(boat, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
        entityManager.SetComponentData(boat, new Position { Value = new float3(0.0f, 0.0f, 0.0f) });
        entityManager.SetComponentData(boat, new Rotation { Value = quaternion.identity });
        entityManager.SetComponentData(boat, new TurnRateComponent { TurnRate = 10 });
        entityManager.SetComponentData(boat, new VelocityComponent { Value = new float3(0, 0, 8) });
    }

    public static void SpawnMeteorite()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var random = new Unity.Mathematics.Random(835483957);

        Entity meteorite = entityManager.CreateEntity(MeteoriteArchetype);
        entityManager.AddSharedComponentData(meteorite, MeteoriteLook);
        entityManager.SetComponentData(meteorite, new Scale { Value = new float3(0.05f, 0.05f, 0.05f) });
        entityManager.SetComponentData(meteorite, new Position { Value = new float3(0.0f, -30f, 0.0f) });
        entityManager.SetComponentData(meteorite, new VelocityComponent { Value = new float3(1f, -10f, 1f) });
        entityManager.SetComponentData(meteorite, new Rotation { Value = Quaternion.identity });
        entityManager.SetComponentData(meteorite, new RotationVelocity { Value = random.NextFloat3() * 300 * new float3(0f, 1f, 0f) - new float3(0f, 150f, 0f)});

        /* Spawn a lot of meteorites */
        for (int i = 0; i < 20; i++)
        {
            meteorite = entityManager.CreateEntity(MeteoriteArchetype);
            entityManager.AddSharedComponentData(meteorite, MeteoriteLook);
            entityManager.SetComponentData(meteorite, new Scale { Value = new float3(0.02f, 0.02f, 0.02f) });
            entityManager.SetComponentData(meteorite, new Position { Value = new float3(0f, -30f, 0f) });
            entityManager.SetComponentData(meteorite, new VelocityComponent { Value = new float3(0f, 0f, 0f) });
            entityManager.SetComponentData(meteorite, new Rotation { Value = Quaternion.identity });
            entityManager.SetComponentData(meteorite, new RotationVelocity { Value = random.NextFloat3() * 300 * new float3(0f, 1f, 0f) - new float3(0f, 150f, 0f) });
        }
    }

    public static void SpawnIslands()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var size = new float3(20.0f, 10.0f, 15.0f);
        var position = new float3(10.0f, 0.0f, 20.0f);

        Entity island = entityManager.CreateEntity(IslandArchetype);
        entityManager.AddSharedComponentData(island, IslandLook);
        entityManager.SetComponentData(island, new Scale { Value = size });
        entityManager.SetComponentData(island, new Position { Value = position });
        entityManager.SetComponentData(island, new Rotation { Value = quaternion.identity });
    }

    public static void SpawnParticles()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var random = new Unity.Mathematics.Random(835483957);
        for (int i = 0; i < 1000; i++)
        {            
            Entity particle = entityManager.CreateEntity(WaterParticleArchetype);
            entityManager.AddSharedComponentData(particle, WaterParticleLook);
            var position = random.NextFloat3() * 100 - Constants.HIGH_WORLD_EDGE;
            position.y = 0;
            var velocity = random.NextFloat3() * 4f - 2;
            velocity.y = 0;

            float lifeTime = random.NextFloat() * Constants.PARTICLE_LIFETIME;

            float scaleX = random.NextFloat() * .3f + 0.3f;
            float scaleZ = random.NextFloat() * 1.5f + 0.8f;

            var angle = random.NextFloat() * 180;

            var q = quaternion.RotateY(angle);

            entityManager.SetComponentData(particle, new Scale { Value = new float3(scaleX, 0.1f, scaleZ)});
            entityManager.SetComponentData(particle, new Position { Value = position });
            entityManager.SetComponentData(particle, new Rotation { Value = q });
            entityManager.SetComponentData(particle, new VelocityComponent { Value = new float3(1, 0, 0) });
            entityManager.SetComponentData(particle, new ParticleComponent { LifeTimeLeft = lifeTime});
        }
    }

    public static void SpawnGoal()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        /*
          // When we have multiple levels, the bunny can be used as well. scale with 100
          entityManager.AddSharedComponentData(goal, BunnyLook);
          entityManager.SetComponentData(goal, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
        */
        var goalPosition = new float3(5, 0, 5);
        Entity goal = entityManager.CreateEntity(GoalArchetype);
        entityManager.AddSharedComponentData(goal, FoxLook);
        entityManager.SetComponentData(goal, new Scale { Value = new float3(2000.0f, 2000.0f, 2000.0f) });
        entityManager.SetComponentData(goal, new Position { Value = goalPosition });
        entityManager.SetComponentData(goal, new Rotation { Value = quaternion.identity });
        entityManager.SetComponentData(goal, new CircleComponent { Position = goalPosition, Radius = 5 });
    }

    public static void NewGame()
    {

        SpawnBoat();
        SpawnMeteorite();
        SpawnIslands();
        SpawnParticles();
        SpawnGoal();
    }

    private static RenderMesh GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<RenderMeshProxy>().Value;
        Object.Destroy(proto);
        return result;
    }
}
