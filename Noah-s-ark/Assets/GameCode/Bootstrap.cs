using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

public sealed class Bootstrap
{
    public static Settings Settings;

    private VectorField vectorField;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        EntitySpawner.Initialize();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        var settingsGO = GameObject.Find("Settings");
        Settings = settingsGO?.GetComponent<Settings>();
        if (!Settings)
            return;

        VectorField.Initialize(Settings);

        EntitySpawner.InitializeWithScene();

        World.Active.GetOrCreateManager<CollisionSystem>().SetupGameObjects();
        World.Active.GetOrCreateManager<InputSystem>().Init();
        World.Active.GetOrCreateManager<WindSystem>().Init();

        NewGame();
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

    public static void NewGame()
    {
        SpawnLevel(1);
        EntitySpawner.SpawnParticles();
        Time.timeScale = 1;
        GameObject.Find("GameStatusText").GetComponent<Text>().text = "";
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
                    EntitySpawner.SpawnIsland(Utils.getCenterOfVectorArea(i,j));
                } else if (pixel == Color.green) //Boat
                {
                    EntitySpawner.SpawnBoat(Utils.getCenterOfVectorArea(i, j));
                } else if (pixel == Color.red) //Goal
                {
                    EntitySpawner.SpawnGoal(Utils.getCenterOfVectorArea(i, j));
                }
            }
        }
        
    }
}
