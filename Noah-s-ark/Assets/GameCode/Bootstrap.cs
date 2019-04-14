using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public sealed class Bootstrap
{
    public static EntityArchetype BoatArchetype;
    public static EntityArchetype WaterParticleArchetype;
    public static EntityArchetype GoalArchetype;
    public static EntityArchetype IslandArchetype;
    public static EntityArchetype MeteoriteArchetype;
    public static EntityArchetype GameStateArchetype;

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

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        
        BoatArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(Rotation), typeof(VelocityComponent), typeof(TurnRateComponent), typeof(Scale), typeof(BoatComponent));
        WaterParticleArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(Scale), typeof(VelocityComponent), typeof(ParticleComponent));
        MeteoriteArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(VelocityComponent), typeof(Scale), typeof(MeteoriteComponent), typeof(Rotation), typeof(RotationVelocity));
        GoalArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(Rotation), typeof(Scale), typeof(GoalComponent));
        IslandArchetype = entityManager.CreateArchetype(typeof(RadiusComponent), typeof(Position), typeof(Rotation), typeof(Scale), typeof(IslandComponent));
        GameStateArchetype = entityManager.CreateArchetype(typeof(GameStateComponent));


    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        Settings = settingsGO?.GetComponent<Settings>();
        if (!Settings)
            return;

        VectorField.Initialize(Settings);


        FoxLook = GetLookFromPrototype("FoxRenderPrototype");
        BunnyLook = GetLookFromPrototype("BunnyRenderPrototype");
        BoatLook = GetLookFromPrototype("BoatRenderPrototype");
        WaterParticleLook = GetLookFromPrototype("WaterParticleRenderPrototype");
        IslandLook = GetLookFromPrototype("IslandRenderPrototype");
        MeteoriteLook = GetLookFromPrototype("MeteoriteRenderPrototype");

        World.Active.GetOrCreateManager<CollisionSystem>().SetupGameObjects();
        World.Active.GetOrCreateManager<InputSystem>().Init();
        World.Active.GetOrCreateManager<WindSystem>().Init();

        NewGame();
    }
    
    public static void SpawnBoat(float3 pos)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        
        Entity boat = entityManager.CreateEntity(BoatArchetype);
        entityManager.AddSharedComponentData(boat, BoatLook);
        entityManager.SetComponentData(boat, new Scale { Value = new float3(100.0f, 100.0f, 100.0f) });
        pos.y = 0.5f; //Make it float
        entityManager.SetComponentData(boat, new Position { Value = pos });
        entityManager.SetComponentData(boat, new Rotation { Value = quaternion.identity });
        entityManager.SetComponentData(boat, new TurnRateComponent { TurnRate = 10 });
        entityManager.SetComponentData(boat, new VelocityComponent { Value = new float3(0, 0, 8) });
        float radius = 4;
        entityManager.SetComponentData(boat, new RadiusComponent { Value = radius });


        var debugMesh = CreateCircleMesh(radius, 100, 0.25f);
        var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
        var debugRender = new DebugRenderComponent { Mesh = debugMesh, Material = debugMaterial };
        entityManager.AddSharedComponentData(boat, debugRender);
    }


    public static void SpawnRandomMeteorites(int amount)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var random = new Unity.Mathematics.Random(835483957);

        /* Spawn a lot of meteorites */
        for (int i = 0; i < amount; i++)
        {
            Entity meteorite = entityManager.CreateEntity(MeteoriteArchetype);
            entityManager.AddSharedComponentData(meteorite, MeteoriteLook);
            entityManager.SetComponentData(meteorite, new Scale { Value = new float3(0.02f, 0.02f, 0.02f) });
            entityManager.SetComponentData(meteorite, new Position { Value = new float3(0f, -30f, 0f) });
            entityManager.SetComponentData(meteorite, new VelocityComponent { Value = new float3(0f, 0f, 0f) });
            entityManager.SetComponentData(meteorite, new Rotation { Value = Quaternion.identity });
            entityManager.SetComponentData(meteorite, new RotationVelocity { Value = random.NextFloat3() * 300 * new float3(0f, 1f, 0f) - new float3(0f, 150f, 0f) });
            float radius = 2;

            var debugMesh = CreateCircleMesh(radius, 100, 0.25f);
            var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
            var debugRender = new DebugRenderComponent { Mesh = debugMesh, Material = debugMaterial };
            entityManager.AddSharedComponentData(meteorite, debugRender);
            entityManager.SetComponentData(meteorite, new RadiusComponent { Value = radius });
        }
    }

    public static void SpawnMeteorite(float3 pos, int amount = 1)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var random = new Unity.Mathematics.Random(835483957);

        /* Spawn a lot of meteorites */
        for (int i = 0; i < amount; i++)
        {
            Entity meteorite = entityManager.CreateEntity(MeteoriteArchetype);
            entityManager.AddSharedComponentData(meteorite, MeteoriteLook);
            entityManager.SetComponentData(meteorite, new Scale { Value = new float3(0.02f, 0.02f, 0.02f) });
            entityManager.SetComponentData(meteorite, new Position { Value = pos + new float3(0f, 40f, 0f) });
            entityManager.SetComponentData(meteorite, new VelocityComponent { Value = new float3(0f, -10f, 0f) });
            entityManager.SetComponentData(meteorite, new Rotation { Value = Quaternion.identity });
            entityManager.SetComponentData(meteorite, new RotationVelocity { Value = random.NextFloat3() * 300 * new float3(0f, 1f, 0f) - new float3(0f, 150f, 0f) });
            float radius = 2;

            var debugMesh = CreateCircleMesh(radius, 100, 0.25f);
            var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
            var debugRender = new DebugRenderComponent { Mesh = debugMesh, Material = debugMaterial };
            entityManager.AddSharedComponentData(meteorite, debugRender);
            entityManager.SetComponentData(meteorite, new RadiusComponent { Value = radius });
        }
    }

    public static void SpawnIsland(float3 pos)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        float radius = 5;

        Entity island = entityManager.CreateEntity(IslandArchetype);
        entityManager.AddSharedComponentData(island, IslandLook);
        entityManager.SetComponentData(island, new Scale { Value = new float3(10.0f, 5.0f, 10.0f) });
        pos.y = Random.Range(-radius*0.3f,0f);
        entityManager.SetComponentData(island, new Position { Value = pos });
        entityManager.SetComponentData(island, new Rotation { Value = quaternion.identity });
        VectorField.Get().AddIsland(pos, radius, 3.5f);

        var debugMesh = CreateCircleMesh(radius, 100, 0.25f);
        var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
        var debugRender = new DebugRenderComponent { Mesh = debugMesh, Material = debugMaterial };
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
            var position = random.NextFloat3() * Constants.MAX_WORLD_SIZE - Constants.HIGH_WORLD_EDGE;
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

    public static void SpawnGoal(float3 pos, int type=0)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        Entity goal = entityManager.CreateEntity(GoalArchetype);
        if (type == 0)
        {
            entityManager.AddSharedComponentData(goal, FoxLook);
            entityManager.SetComponentData(goal, new Scale { Value = new float3(1700.0f, 1700.0f, 1700.0f) });
        }
        else
        {
            entityManager.AddSharedComponentData(goal, BunnyLook);
            entityManager.SetComponentData(goal, new Scale { Value = new float3(50.0f, 50.0f, 50.0f) });
        }
        entityManager.SetComponentData(goal, new Position { Value = pos });
        entityManager.SetComponentData(goal, new Rotation { Value = quaternion.identity });
        float radius = 3;
        entityManager.SetComponentData(goal, new RadiusComponent { Value = radius });

        var debugMesh = CreateCircleMesh(radius, 100, 0.25f);
        var debugMaterial = new Material(Shader.Find("Unlit/DebugShader"));
        var debugRender = new DebugRenderComponent { Mesh = debugMesh, Material = debugMaterial };
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
        VectorField.Reset();

    }

    public static void NewGame(int level = 1)
    {
        Debug.Log("Level: " + level);
        SpawnLevel(level);
        SpawnParticles();
        Time.timeScale = 1;
        GameObject.Find("GameStatusText").GetComponent<Text>().text = "";

        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        Entity gameState = entityManager.CreateEntity(GameStateArchetype);
        entityManager.AddComponentData(gameState, new GameStateComponent { currentLevel = 1 });
    }

    private static Mesh CreateCircleMesh(float radius, int numberOfSides, float thickness)
    {
        //verticies
        var verticies = new Vector3[numberOfSides * 2];
        float x;
        float y;
        for (int i = 0; i < numberOfSides; i++)
        {
            x = radius * Mathf.Sin((2 * Mathf.PI * i) / numberOfSides);
            y = radius * Mathf.Cos((2 * Mathf.PI * i) / numberOfSides);
            verticies[i] = new Vector3(x, 0, y);
        }
        for (int i = 0; i < numberOfSides; i++)
        {
            x = (radius - thickness) * Mathf.Sin((2 * Mathf.PI * i) / numberOfSides);
            y = (radius - thickness) * Mathf.Cos((2 * Mathf.PI * i) / numberOfSides);
            verticies[numberOfSides + i] = new Vector3(x, 0, y);
        }


        //triangles
        var triangles = new int[numberOfSides * 6 + 6];
        int triangleIndex = 0;
        for (int i = 0; i < (numberOfSides - 1); i++)
        {
            triangles[triangleIndex] = i;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = numberOfSides + i;

            triangles[triangleIndex + 3] = numberOfSides + i;
            triangles[triangleIndex + 4] = i + 1;
            triangles[triangleIndex + 5] = numberOfSides + i + 1;

            triangleIndex += 6;
        }

        //Add the bit triangles to connect the edges
        triangles[triangleIndex] = numberOfSides - 1;
        triangles[triangleIndex + 1] = 0;
        triangles[triangleIndex + 2] = numberOfSides*2 -1;

        triangles[triangleIndex + 3] = numberOfSides*2 -1;
        triangles[triangleIndex + 4] = 0;
        triangles[triangleIndex + 5] = numberOfSides;

        //normals
        var normals = new Vector3[numberOfSides *2];
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

    private static Mesh CreateSphereMesh(float radius, int numberOfSides, float thickness)
    {
        //verticies
        var verticies = new Vector3[numberOfSides * 2];
        float x;
        float y;
        for (int i = 0; i < numberOfSides; i++)
        {
            x = radius * Mathf.Sin((2 * Mathf.PI * i) / numberOfSides);
            y = radius * Mathf.Cos((2 * Mathf.PI * i) / numberOfSides);
            verticies[i] = new Vector3(x, 0, y);
        }
        for (int i = 0; i < numberOfSides; i++)
        {
            x = (radius - thickness) * Mathf.Sin((2 * Mathf.PI * i) / numberOfSides);
            y = (radius - thickness) * Mathf.Cos((2 * Mathf.PI * i) / numberOfSides);
            verticies[numberOfSides + i] = new Vector3(x, 0, y);
        }


        //triangles
        var triangles = new int[numberOfSides * 6 + 6];
        int triangleIndex = 0;
        for (int i = 0; i < (numberOfSides - 1); i++)
        {
            triangles[triangleIndex] = i;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = numberOfSides + i;

            triangles[triangleIndex + 3] = numberOfSides + i;
            triangles[triangleIndex + 4] = i + 1;
            triangles[triangleIndex + 5] = numberOfSides + i + 1;

            triangleIndex += 6;
        }

        //Add the bit triangles to connect the edges
        triangles[triangleIndex] = numberOfSides - 1;
        triangles[triangleIndex + 1] = 0;
        triangles[triangleIndex + 2] = numberOfSides * 2 - 1;

        triangles[triangleIndex + 3] = numberOfSides * 2 - 1;
        triangles[triangleIndex + 4] = 0;
        triangles[triangleIndex + 5] = numberOfSides;

        //normals
        var normals = new Vector3[numberOfSides * 2];
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

    private static void SpawnLevel(int level)
    {
        Texture2D image = Resources.Load<Texture2D>("level" + level);
        for (int i = 0; i < image.width; i++)
        {
            for (int j = 0; j < image.height; j++)
            {
                Color pixel = image.GetPixel(i, j);
                if (pixel == Color.black) //Islands
                {
                    SpawnIsland(Utils.getCenterOfVectorArea(i,j));
                } else if (pixel == Color.green) //Boat
                {
                    SpawnBoat(Utils.getCenterOfVectorArea(i, j));
                } else if (pixel == Color.red) //Goal
                {
                    SpawnGoal(Utils.getCenterOfVectorArea(i, j));
                }
            }
        }
        
    }
}
