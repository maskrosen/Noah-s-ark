using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class VectorField
{
    private static VectorField instance;

    /*
     * A 2D array of Vector2, stored as one for PERFORMANCE MAXIMUS. Access with field[i * Constants.VECTORFIELD_SIZE + j]
    */
    public readonly Vector2[] field;

    public static void Initialize()
    {
        instance = new VectorField(new Vector2[Constants.VECTORFIELD_SIZE * Constants.VECTORFIELD_SIZE]);

        for (int i = 0; i < Constants.VECTORFIELD_SIZE; i++)
        {
            for (int j = 0; j < Constants.VECTORFIELD_SIZE; j++)
            {
                instance.field[i * Constants.VECTORFIELD_SIZE + j] = defaultVectorField(i - Constants.VECTORFIELD_SIZE/2, j - Constants.VECTORFIELD_SIZE / 2);
            }
        }
    }

    private static Vector2 defaultVectorField(float i, float j)
    {
        
        Vector2 v = new Vector2(i + 5, j + 5) / Constants.VECTORFIELD_SIZE;
        v += new Vector2(-v.y, v.x) * 3/v.magnitude;
        return v;
    }


    public static VectorField Get()
    {
        if(instance == null)
        {
            throw new System.Exception("Vector field not serialized");
        }
        return instance;
    }

    private VectorField(Vector2[] field)
    {
        this.field = field;
    }

    public Vector2 VectorAtPos(float3 position)
    {
        float fi = Mathf.Clamp(position.x + Constants.VECTORFIELD_SIZE / 2, 0, Constants.VECTORFIELD_SIZE-1);
        float fj = Mathf.Clamp(position.z + Constants.VECTORFIELD_SIZE / 2, 0, Constants.VECTORFIELD_SIZE-1);

        int i = (int)fi;
        int j = (int)fj;

        //interpolate

        //todo lerp samples based fractional difference
        return field[i * Constants.VECTORFIELD_SIZE + j];
    }
}
