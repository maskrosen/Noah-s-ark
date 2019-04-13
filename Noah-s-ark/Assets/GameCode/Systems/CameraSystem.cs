using UnityEngine;
using UnityEditor;
using Unity.Entities;
using Unity.Transforms;

public class CameraSystem : ComponentSystem
{
    public struct CameraData
    {
        public readonly int Length;
        public ComponentDataArray<BoatComponent> BoatComponent;
        public ComponentDataArray<Position> BoatPosition;
    }

    [Inject] private CameraData cameraData;

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < cameraData.Length; i++)
        {

            /* Set fixed Camera Angle and Rotation. */
            //Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //Camera.main.transform.position = new Vector3(0f, 0f, 0f);


            /* Make the camera look towards the Boat. */
            Camera.main.transform.LookAt(cameraData.BoatPosition[i].Value);

        }
    }
}