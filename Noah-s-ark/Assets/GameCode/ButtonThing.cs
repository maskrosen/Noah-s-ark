using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ButtonThing : MonoBehaviour
{

    private bool waitingForClick = false;
    private string currentPower = null;
    private Dictionary<string, float> lastPowerUse;
    private Dictionary<string, float> powerCooldown;

    private void Start()
    {
        float time_of_origin = Time.realtimeSinceStartup;
        lastPowerUse = new Dictionary<string, float>();
        powerCooldown = new Dictionary<string, float>();
        lastPowerUse.Add("bunny", -100000f);
        lastPowerUse.Add("meteorite", -100000f);
        lastPowerUse.Add("whirlpool", -100000f);

        powerCooldown.Add("bunny", 3f);
        powerCooldown.Add("meteorite", 5f);
        powerCooldown.Add("whirlpool", 3f);

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
                
                float currentTime = Time.realtimeSinceStartup;
                bool powerOnCooldown = currentTime - lastPowerUse[currentPower] < powerCooldown[currentPower];

                /* == If you want to debug the CD feature. ==
                Debug.Log("CurrentTime: " + currentTime);
                Debug.Log("Power On Cooldown: " + powerOnCooldown);
                Debug.Log("Time elapsed: " + (currentTime - lastPowerUse[currentPower]));
                Debug.Log("Power CD: " + powerCooldown[currentPower]);
                */


                if (!powerOnCooldown)
                {
                    var goalPosition = new float3(hitPoint.x, 0, hitPoint.z);
                    if (currentPower == "bunny")
                    {
                        Debug.Log("Generating bunny");
                        EntitySpawner.SpawnGoal(goalPosition, 1);
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
                        EntitySpawner.SpawnMeteorite(goalPosition, 1);
                    }

                    lastPowerUse[currentPower] = Time.realtimeSinceStartup;
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
