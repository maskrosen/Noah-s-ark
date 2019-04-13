using UnityEngine;
using System.Collections;

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
                instance.field[i * Constants.VECTORFIELD_SIZE + j] = Vector2.up;
            }
        }

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
}
