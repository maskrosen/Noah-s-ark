using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine.UI;

public sealed class Bootstrap
{
    public static EntityArchetype BoatArchetype;
    public static EntityArchetype WaterParticleArchetype;
    public static EntityArchetype GoalArchetype;
    public static EntityArchetype IslandArchetype;

    public static RenderMesh FoxLook;
    public static RenderMesh BunnyLook;
    public static RenderMesh BoatLook;
    public static RenderMesh WaterParticleLook;
    public static RenderMesh IslandLook;

    public static Settings Settings;

    private VectorField vectorField;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        VectorField.Initialize();

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        
        BoatArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(TurnRateComponent), typeof(Scale), typeof(BoatComponent));
        WaterParticleArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Scale), typeof(VelocityComponent), typeof(ParticleComponent));

        GoalArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(Rotation), typeof(Scale), typeof(GoalComponent));
        IslandArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(Rotation), typeof(Scale), typeof(IslandComponent));

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

        World.Active.GetOrCreateManager<CollisionSystem>().SetupGameObjects();
        NewGame();
    }


    public static void SpawnBoat()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity boat = entityManager.CreateEntity(BoatArchetype);
        entityManager.AddSharedComponentData(boat, BoatLook);
        entityManager.SetComponentData(boat, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
        entityManager.SetComponentData(boat, new Position { Value = new float3(0.0f, 0.5f, 0.0f) });
        entityManager.SetComponentData(boat, new Rotation { Value = quaternion.identity });
        entityManager.SetComponentData(boat, new TurnRateComponent { TurnRate = 10 });
        entityManager.SetComponentData(boat, new VelocityComponent { Value = new float3(0, 0, 8) });
        float radius = 4;
        entityManager.SetComponentData(boat, new RadiusComponent { Value = radius });


        var debugMesh = CreateCircleMesh(radius, 100);
        var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
        var debugRender = new DebugRenderComponent { mesh = debugMesh, material = debugMaterial };
        entityManager.AddSharedComponentData(boat, debugRender);
    }

    public static void SpawnIslands()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity island = entityManager.CreateEntity(IslandArchetype);
        entityManager.AddSharedComponentData(island, IslandLook);
        entityManager.SetComponentData(island, new Scale { Value = new float3(10.0f, 5.0f, 10.0f) });
        entityManager.SetComponentData(island, new Position { Value = new float3(0.0f, 0.0f, 20.0f) });
        entityManager.SetComponentData(island, new Rotation { Value = quaternion.identity });
        float radius = 5;

        var debugMesh = CreateCircleMesh(radius, 100);
        var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
        var debugRender = new DebugRenderComponent { mesh = debugMesh, material = debugMaterial };
        entityManager.AddSharedComponentData(island, debugRender);
        entityManager.SetComponentData(island, new RadiusComponent { Value = radius });
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

        var goalPosition = new float3(-10, 0, 20);
        Entity goal = entityManager.CreateEntity(GoalArchetype);
        entityManager.AddSharedComponentData(goal, FoxLook);
        entityManager.SetComponentData(goal, new Scale { Value = new float3(2000.0f, 2000.0f, 2000.0f) });
        entityManager.SetComponentData(goal, new Position { Value = goalPosition });
        entityManager.SetComponentData(goal, new Rotation { Value = quaternion.identity });
        float radius = 5;
        entityManager.SetComponentData(goal, new RadiusComponent { Value = radius });

        var debugMesh = CreateCircleMesh(radius, 100);
        var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
        var debugRender = new DebugRenderComponent { mesh = debugMesh, material = debugMaterial };
        entityManager.AddSharedComponentData(goal, debugRender);

    }

    public static void ClearGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var entities = entityManager.GetAllEntities();
        foreach (Entity e in entities)
        {
            entityManager.DestroyEntity(e);
        }
    }

    public static void NewGame()
    {
        SpawnBoat();
        SpawnIslands();
        SpawnParticles();
        SpawnGoal();
        Time.timeScale = 1;
        GameObject.Find("GameStatusText").GetComponent<Text>().text = "";
    }

    private static Mesh CreateCircleMesh(float radius, int numberOfSides)
    {
        //verticies
        var verticies = new Vector3[numberOfSides];
        float x;
        float y;
        for (int i = 0; i < numberOfSides; i++)
        {
            x = radius * Mathf.Sin((2 * Mathf.PI * i) / numberOfSides);
            y = radius * Mathf.Cos((2 * Mathf.PI * i) / numberOfSides);
            verticies[i] = new Vector3(x, 0, y);
        }


        //triangles
        var triangles = new int[numberOfSides * 3];
        int triangleIndex = 0;
        for (int i = 0; i < (numberOfSides - 2); i++)
        {
            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = i + 2;
            triangleIndex += 3;
        }

        //normals
        var normals = new Vector3[numberOfSides];
        for (int i = 0; i < verticies.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        var mesh = new Mesh();
        //initialise
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

    private static RenderMesh GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<RenderMeshProxy>().Value;
        Object.Destroy(proto);
        return result;
    }
}
