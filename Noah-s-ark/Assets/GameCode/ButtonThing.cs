using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ButtonThing : MonoBehaviour
{

    private bool waitingForClick = false;
    private string currentPower = null;

    private void Start()
    {
    }

    private void Update()
    {
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            // this creates a horizontal plane passing through this object's center
            var plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
            // create a ray from the mousePosition
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // plane.Raycast returns the distance from the ray start to the hit point
            float distance = 0;
            if (plane.Raycast(ray, out distance))
            {
                // some point of the plane was hit - get its coordinates
                var hitPoint = ray.GetPoint(distance);
                // use the hitPoint to aim your cannon
                //Debug.Log("hitPoint: " + hitPoint);

                var goalPosition = new float3(hitPoint.x, 0, hitPoint.z);
                if (currentPower == "bunny")
                {
                    Debug.Log("Generating bunny");
                    Bootstrap.SpawnGoal(goalPosition, 1);
                }
                else if (currentPower == "whirlpool")
                {
                    Debug.Log("Generating whirlpool");
                    bool clockwise = UnityEngine.Random.Range(0f, 1f) > 0.5;
                    VectorField.Get().AddWhirlpool(goalPosition, 10, clockwise, 20);
                }
                else if (currentPower == "meteorite")
                {
                    Debug.Log("Generating Meteorite");
                    Bootstrap.SpawnMeteorite(goalPosition, 1);
                }
            }
            waitingForClick = false;
            currentPower = null;
        }
    }

    public void OnClickPower(string buttonName)
    {
        waitingForClick = true;
        currentPower = buttonName;
        Debug.Log("Waiting for click");
    }

    public void OnClickRestart()
    {
        Bootstrap.ClearGame();
        Bootstrap.NewGame();
    }

    public void OnClickFF()
    {
        if (Time.timeScale > 1)
        {
            Time.timeScale = 1;
        } else
        {
            Time.timeScale = 4;
        }
    }

    public void OnWindClicked()
    {

    }
}
