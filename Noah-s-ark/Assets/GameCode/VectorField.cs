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

    private Settings settings;

    public static void Initialize(Settings settings)
    {
        instance = new VectorField(new Vector2[Constants.VECTORFIELD_SIZE * Constants.VECTORFIELD_SIZE]);
        instance.settings = settings;

        for (int i = 0; i < Constants.VECTORFIELD_SIZE; i++)
        {
            for (int j = 0; j < Constants.VECTORFIELD_SIZE; j++)
            {
                instance.field[i * Constants.VECTORFIELD_SIZE + j] = defaultVectorField(i - Constants.VECTORFIELD_SIZE/2, j - Constants.VECTORFIELD_SIZE / 2);
            }
        }
        instance.AddWhirlpool(new float3(30,0,30), 15, true, 30);
        instance.AddWhirlpool(new float3(-30, 0, -30), 15, false, 30);
    }

    private static Vector2 defaultVectorField(float i, float j)
    {
        Vector2 v = new Vector2(0.1f, 2);
        
        return v;
    }

    public void AddWhirlpool(float3 pos, float str, bool clockwise, float eventHorizonRadius)
    {
        float x = Mathf.Clamp(pos.x + Constants.WORLD_VECTORFIELD_OFFSET, 0, Constants.VECTORFIELD_SIZE - 1);
        float y = Mathf.Clamp(pos.z + Constants.WORLD_VECTORFIELD_OFFSET, 0, Constants.VECTORFIELD_SIZE - 1);

        float dir = clockwise ? 1 : -1;

        float maxMag = Mathf.Sqrt(Constants.VECTORFIELD_SIZE / 2 * Constants.VECTORFIELD_SIZE / 2 + Constants.VECTORFIELD_SIZE / 2 * Constants.VECTORFIELD_SIZE / 2);

        for (int i = 0; i < Constants.VECTORFIELD_SIZE; i++)
        {
            for (int j = 0; j < Constants.VECTORFIELD_SIZE; j++)
            {
                Vector2 v = new Vector2(i - x, j - y) * str / Constants.VECTORFIELD_SIZE;
                v += new Vector2(-v.y, v.x) * 3 * dir;
                
                Vector2 toCenter = new Vector2(x, y) - new Vector2(i, j);

                float distToCenter = toCenter.magnitude / eventHorizonRadius;
                field[i * Constants.VECTORFIELD_SIZE + j] = Vector2.Lerp(-v, field[i * Constants.VECTORFIELD_SIZE + j], Mathf.Sqrt(Mathf.Clamp01(distToCenter)));
            }
        }
    }

    public void AddIsland(float3 pos, float radius, float ExtraAffectRadius)
    {
        float x = Mathf.Clamp(pos.x + Constants.WORLD_VECTORFIELD_OFFSET, 0, Constants.VECTORFIELD_SIZE - 1);
        float y = Mathf.Clamp(pos.z + Constants.WORLD_VECTORFIELD_OFFSET, 0, Constants.VECTORFIELD_SIZE - 1);

        for (int i = 0; i < Constants.VECTORFIELD_SIZE; i++)
        {
            for (int j = 0; j < Constants.VECTORFIELD_SIZE; j++)
            {
                Vector2 toCenter = new Vector2(x, y) - new Vector2(i, j);
                float distToCenter = Mathf.Clamp01(toCenter.magnitude / (radius + ExtraAffectRadius));

                if (toCenter.magnitude < radius)
                    field[i * Constants.VECTORFIELD_SIZE + j] = Vector2.zero;
                else
                    field[i * Constants.VECTORFIELD_SIZE + j] = Vector2.Lerp(-toCenter, field[i * Constants.VECTORFIELD_SIZE + j], Mathf.Sqrt(distToCenter));
            }
        }

    }
    
    public static VectorField Get()
    {
        if(instance == null)
        {
            throw new System.Exception("Vector field not initialized");
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
        Vector2 ud = Vector2.Lerp(d, u, pos01.y);

        return ud;
    }

    public Vector2 VectorAtPos(float3 position)
    {
        float fi = Mathf.Clamp(position.x + Constants.WORLD_VECTORFIELD_OFFSET, 0, Constants.VECTORFIELD_SIZE-1);
        float fj = Mathf.Clamp(position.z + Constants.WORLD_VECTORFIELD_OFFSET, 0, Constants.VECTORFIELD_SIZE-1);

        int i = (int)fi;
        int j = (int)fj;

        float ifrac = fi - i;
        float jfrac = fj - j;

        //return field[i*Constants.VECTORFIELD_SIZE + j];
        
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
        else if(ifrac <= 0.5f && jfrac > 0.5f)
        {
            pos01 = new Vector2(ifrac + 0.5f, jfrac - 0.5f);

            r = Mathf.Clamp(i, 0, Constants.VECTORFIELD_SIZE - 1);
            l = Mathf.Clamp(i - 1, 0, Constants.VECTORFIELD_SIZE - 1);
            
            u = Mathf.Clamp(j + 1, 0, Constants.VECTORFIELD_SIZE - 1);
            d = Mathf.Clamp(j, 0, Constants.VECTORFIELD_SIZE - 1);
            
        }
        else if(ifrac > 0.5f && jfrac <= 0.5f)
        {
            pos01 = new Vector2(ifrac - 0.5f, jfrac + 0.5f);

            r = Mathf.Clamp(i + 1, 0, Constants.VECTORFIELD_SIZE - 1);
            l = Mathf.Clamp(i, 0, Constants.VECTORFIELD_SIZE - 1);

            u = Mathf.Clamp(j, 0, Constants.VECTORFIELD_SIZE - 1);
            d = Mathf.Clamp(j - 1, 0, Constants.VECTORFIELD_SIZE - 1);

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
