using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class Settings : MonoBehaviour
{


    public float playerRotationSpeed = 10.0f;
    public float3 playerRotation = new float3(0,1,0);
    public float playerSpaceDistance = 15f;
    public float playerOffset = 15f;

    public float distanceBetweenPlayers = 15f;

    public int playerStandingSpots = 3;

    public float botTurnCooldown = 2f;

    public float bulletSpeed = 10f;
    public float bulletLifeDuration = 3f;

    public float cameraSpeed = 1f;
    public float cameraDistance = 25f;
    public float cameraHeight = 15f;

    public bool debugMode = true;


}
