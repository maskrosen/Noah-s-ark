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

            var x = cameraData.BoatPosition[i].Value.x;
            var y = cameraData.BoatPosition[i].Value.y;
            var z = cameraData.BoatPosition[i].Value.z;

            /* 
             * PLEASE OBSERVE.
             * De-comment / comment out whichever view you want to have below.
             */

            /* Set fixed Camera Angle and Rotation in center of map. */
            //Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //Camera.main.transform.position = new Vector3(0f, 0f, 0f);

            /* Fixed top down view */
            Camera.main.transform.position = new Vector3(0f, 30f, 0f);
            Camera.main.transform.forward = Vector3.down;

            /* Top down view, centered over Boat. */
            //Camera.main.transform.position = new Vector3(x, 30f, z);
            //Camera.main.transform.forward = Vector3.down;

            /* Set a third person view. */
            //Camera.main.transform.position = new Vector3(20, y + 10, z + 12);


            /* GENERAL SETTING: 
             * Make the camera look towards the Boat (current camera position, Boat centered in view). 
             */
            //Camera.main.transform.LookAt(cameraData.BoatPosition[i].Value);

        }
    }
}