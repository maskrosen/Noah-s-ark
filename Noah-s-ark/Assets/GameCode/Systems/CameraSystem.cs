using UnityEngine;
using UnityEditor;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CameraSystem : ComponentSystem
{
    public struct CameraData
    {
        public readonly int Length;
        public ComponentDataArray<BoatComponent> BoatComponent;
        public ComponentDataArray<Position> BoatPosition;
        public ComponentDataArray<VelocityComponent> Velocity;
    }

    private Vector3 boatPrevPos = new Vector3(0f, 0f, 0f);
    private const float distance = 25f;

    [Inject] private CameraData cameraData;

    protected override void OnStartRunning()
    {
        Camera.main.transform.position = new Vector3(20f, 20f, 20f);
    }

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < cameraData.Length; i++)
        {

            var x = cameraData.BoatPosition[i].Value.x;
            var y = cameraData.BoatPosition[i].Value.y;
            var z = cameraData.BoatPosition[i].Value.z;
            
            var boatPos = new Vector3(x, y, z);

            /* 
             * PLEASE OBSERVE.
             * De-comment / comment out whichever view you want to have below.
             */

            /* Set fixed Camera Angle and Rotation in center of map. */
            //Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //Camera.main.transform.position = new Vector3(0f, 0f, 0f);

            /* Fixed top down view */
            //Camera.main.transform.position = new Vector3(0f, 30f, 0f);
            //Camera.main.transform.forward = Vector3.down;

            /* Top down view, centered over Boat. */
            //Camera.main.transform.position = new Vector3(x, 30f, z);
            //Camera.main.transform.forward = Vector3.down;

            /* Set a third person view. */
            //Camera.main.transform.position = new Vector3(20, y + 10, z + 12);

            /* Third person view from behind the boat. */
            var boatMoveDir = boatPos - boatPrevPos;
            if (boatMoveDir != Vector3.zero)
            {
                Camera.main.transform.position = boatPos - boatMoveDir.normalized * distance + Vector3.up * 10f;
                Camera.main.transform.LookAt(boatPos);
                boatPrevPos = boatPos;
            }

            /* GENERAL SETTING: 
             * Make the camera look towards the Boat (current camera position, Boat centered in view). 
             */
            //Camera.main.transform.LookAt(cameraData.BoatPosition[i].Value);

        }
    }
}