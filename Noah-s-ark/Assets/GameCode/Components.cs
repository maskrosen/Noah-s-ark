using UnityEngine;
using Unity.Entities;
using System;
using Unity.Mathematics;

public struct VelocityComponent : IComponentData
{
    public float3 Value;
}

public struct TurnRateComponent : IComponentData
{
    public float TurnRate;
}

public struct RotationVelocity : IComponentData
{
    public float3 Value;
}

public struct TimerComponent : IComponentData
{
    public float CurrentTime;
    public float Duration;
    public BlittableBool DeleteOnEnd;
}

public struct ParticleComponent : IComponentData
{
    public float LifeTimeLeft;
}
    
public struct BoatComponent : IComponentData
{
}

public struct GoalComponent : IComponentData
{

}

public struct IslandComponent : IComponentData
{

}

public struct MeteoriteComponent : IComponentData
{

}

public struct RadiusComponent : IComponentData

{
    public float Value;
}

public struct DebugRenderComponent : ISharedComponentData
{
    public Mesh mesh;
    public Material material;
}

public struct BlittableBool : IEquatable<BlittableBool>
{
    private byte value;

    public BlittableBool(bool value)
    {
        this.value = Convert.ToByte(value);
    }

    public static implicit operator bool(BlittableBool blittableBool)
    {
        return blittableBool.value != 0;
    }

    public static implicit operator BlittableBool(bool value)
    {
        return new BlittableBool(value);
    }

    public bool Equals(BlittableBool other)
    {
        return value == other.value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        return obj is BlittableBool && Equals((BlittableBool)obj);
    }

    public override int GetHashCode()
    {
        return value;
    }

    public static bool operator ==(BlittableBool left, BlittableBool right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlittableBool left, BlittableBool right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return ((bool)this).ToString();
    }
}

