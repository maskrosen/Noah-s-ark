using UnityEngine;
using Unity.Entities;
using System;
using Unity.Mathematics;


public struct PlayerPosition : IComponentData
{
    public int Position;
}

public struct PlayerInput : IComponentData
{
    public int MoveDirection;
    public BlittableBool Shoot;
}

public struct PlayerTurnState : IComponentData
{
    public BlittableBool PlayersTurn;
    public BlittableBool TurnDone;
}

public struct PlayerFaction : IComponentData
{
    public int Faction;
}

public struct GameState : IComponentData
{
    public int CurrentTurnFaction;
    public int CurrentState;
}

public struct BotState : IComponentData
{
    public float TurnCooldown;
}

public struct VelocityComponent : IComponentData
{
    public float3 Velocity; 
}

public struct TurnRateComponent : IComponentData
{
    public float TurnRate;
}

public struct TimerComponent : IComponentData
{
    public float CurrentTime;
    public float Duration;
    public BlittableBool DeleteOnEnd;
}

public struct ParticleComponent : IComponentData
{
}
    
public struct BoatComponent : IComponentData
{
}

public struct PlayerComponent : IComponentData
{

}

public struct BulletComponent : IComponentData
{

}

public struct CircleComponent : IComponentData
{
    public float3 Position;
    public float Radius;
}

public struct GameOverComponent : IComponentData
{
    public int WinningFaction;
}

public struct GameStates
{
    public const int Menu = 0;
    public const int Playing = 1;
    public const int GameOver = 2;
}

public struct Factions
{
    public const int Player = 0;
    public const int Enemy = 1;
    public const int None = -1;

    public static int OtherPlayer(int playerFaction)
    {
        if (playerFaction == Player)
            return Enemy;
        else if (playerFaction == Enemy)
            return Player;
        return -1;
    }

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

