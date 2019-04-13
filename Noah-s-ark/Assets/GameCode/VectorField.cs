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

    private Vector2 lerpSample(Vector2 tr, Vector2 tl, Vector2 br, Vector2 bl, Vector2 pos01)
    {
        Vector2 u = Vector2.Lerp(tl, tr, pos01.x);
        Vector2 d = Vector2.Lerp(bl, br, pos01.x);
        //Vector2 l = Vector2.Lerp(bl, tl, pos01.y);
        //Vector2 r = Vector2.Lerp(br, tr, pos01.y);

        Vector2 ud = Vector2.Lerp(d, u, pos01.y);
        //Vector2 lr = Vector2.Lerp(l, r, pos01.x);
        return ud;
    }

    public Vector2 VectorAtPos(float3 position)
    {
        float fi = Mathf.Clamp(position.x + Constants.VECTORFIELD_SIZE / 2, 0, Constants.VECTORFIELD_SIZE-1);
        float fj = Mathf.Clamp(position.z + Constants.VECTORFIELD_SIZE / 2, 0, Constants.VECTORFIELD_SIZE-1);

        int i = (int)fi;
        int j = (int)fj;

        float ifrac = fi % i;
        float jfrac = fj % j;

        
        Vector2 pos01;
        
        int r;
        int l;

        int u;
        int d;

        if (ifrac > 0.5f && jfrac > 0.5f)
        {
            pos01 = new Vector2(ifrac - 0.5f, jfrac - 0.5f);

            r = Mathf.Clamp(i + 1, 0, Constants.VECTORFIELD_SIZE - 1);
            l = Mathf.Clamp(i, 0, Constants.VECTORFIELD_SIZE - 1);

            u = Mathf.Clamp(j + 1, 0, Constants.VECTORFIELD_SIZE - 1);
            d = Mathf.Clamp(j, 0, Constants.VECTORFIELD_SIZE - 1);

        }
        else if(ifrac < 0.5f && jfrac > 0.5f)
        {
            pos01 = new Vector2(ifrac + 0.5f, jfrac - 0.5f);

            r = Mathf.Clamp(i, 0, Constants.VECTORFIELD_SIZE - 1);
            l = Mathf.Clamp(i - 1, 0, Constants.VECTORFIELD_SIZE - 1);
            
            u = Mathf.Clamp(j + 1, 0, Constants.VECTORFIELD_SIZE - 1);
            d = Mathf.Clamp(j, 0, Constants.VECTORFIELD_SIZE - 1);


        }
        else if(ifrac > 0.5f && jfrac < 0.5f)
        {
            pos01 = new Vector2(ifrac - 0.5f, jfrac + 0.5f);

            r = Mathf.Clamp(i, 0, Constants.VECTORFIELD_SIZE - 1);
            l = Mathf.Clamp(i - 1, 0, Constants.VECTORFIELD_SIZE - 1);

            u = Mathf.Clamp(j + 1, 0, Constants.VECTORFIELD_SIZE - 1);
            d = Mathf.Clamp(j, 0, Constants.VECTORFIELD_SIZE - 1);

        }
        else //if (ifrac < 0.5f && jfrac < 0.5f)
        {
            pos01 = new Vector2(ifrac + 0.5f, jfrac + 0.5f);

            r = Mathf.Clamp(i, 0, Constants.VECTORFIELD_SIZE - 1);
            l = Mathf.Clamp(i - 1, 0, Constants.VECTORFIELD_SIZE - 1);

            u = Mathf.Clamp(j, 0, Constants.VECTORFIELD_SIZE - 1);
            d = Mathf.Clamp(j - 1, 0, Constants.VECTORFIELD_SIZE - 1);
        }

        Vector2 urV = field[r * Constants.VECTORFIELD_SIZE + u];
        Vector2 ulV = field[l * Constants.VECTORFIELD_SIZE + u];

        Vector2 drV = field[r * Constants.VECTORFIELD_SIZE + d];
        Vector2 dlV = field[l * Constants.VECTORFIELD_SIZE + d];

        Vector2 sample = lerpSample(urV, ulV, drV, dlV, pos01);

        return sample;
    }
}
