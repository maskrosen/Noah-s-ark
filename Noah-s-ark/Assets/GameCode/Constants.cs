using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants
{
    public const int VECTORFIELD_SIZE = 200;
    public const int MAX_WORLD_SIZE = VECTORFIELD_SIZE; //same as vectorfield size for simplicty atm

    public const float LOW_WORLD_EDGE = -VECTORFIELD_SIZE/2;
    public const float HIGH_WORLD_EDGE = VECTORFIELD_SIZE/2;
    public const int WORLD_VECTORFIELD_OFFSET = VECTORFIELD_SIZE/2;

    public const float PARTICLE_LIFETIME = 60;
    public const float WIND_POWERUP_TIME = 10;
    public const float WIND_POWERUP_SPEED = 15;

    public const string BUNNY_BTN = "bunny";
    public const string METEORITE_BTN = "meteorite";
    public const string WHIRLPOOL_BTN = "whirlpool";

}
