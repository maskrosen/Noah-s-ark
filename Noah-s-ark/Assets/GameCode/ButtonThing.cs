using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonThing : MonoBehaviour
{

    private bool waitingForClick = false;
    private string currentPower = null;
    private Dictionary<string, float> lastPowerUse;
    private Dictionary<string, float> powerCooldown;
    private Dictionary<string, string> buttonGameObjectNames;

    private void Start()
    {
        var settings = Bootstrap.Settings;

        lastPowerUse = new Dictionary<string, float>();
        powerCooldown = new Dictionary<string, float>();
        buttonGameObjectNames = new Dictionary<string, string>();

        lastPowerUse.Add(Constants.BUNNY_BTN, -100000f);
        lastPowerUse.Add(Constants.METEORITE_BTN, -100000f);
        lastPowerUse.Add(Constants.WHIRLPOOL_BTN, -100000f);

        powerCooldown.Add(Constants.BUNNY_BTN, settings.bunnyCD);
        powerCooldown.Add(Constants.METEORITE_BTN, settings.meteoriteCD);
        powerCooldown.Add(Constants.WHIRLPOOL_BTN, settings.whirlpoolCD);

        buttonGameObjectNames.Add(Constants.BUNNY_BTN, "btn4");
        buttonGameObjectNames.Add(Constants.METEORITE_BTN, "btn2");
        buttonGameObjectNames.Add(Constants.WHIRLPOOL_BTN, "btn3");

    }

    private void Update()
    {
        /* Get updated CD settings. */
        var settings = Bootstrap.Settings;
        powerCooldown[Constants.BUNNY_BTN] = settings.bunnyCD;
        powerCooldown[Constants.METEORITE_BTN] = settings.meteoriteCD;
        powerCooldown[Constants.WHIRLPOOL_BTN] = settings.whirlpoolCD;

        /* Handle the click. */
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
                    if (currentPower == Constants.BUNNY_BTN)
                    {
                        Debug.Log("Generating bunny");
                        EntitySpawner.SpawnGoal(goalPosition, 1);
                    }
                    else if (currentPower == Constants.WHIRLPOOL_BTN)
                    {
                        Debug.Log("Generating whirlpool");
                        bool clockwise = UnityEngine.Random.Range(0f, 1f) > 0.5;
                        VectorField.Get().AddWhirlpool(goalPosition, 10, clockwise, 20);
                    }
                    else if (currentPower == Constants.METEORITE_BTN)
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

        updateButtonCooldowns();
    }

    public void updateButtonCooldowns()
    {
        foreach (KeyValuePair<string, string> item in buttonGameObjectNames)
        {
            float currentTime = Time.realtimeSinceStartup;
            bool powerOnCooldown = currentTime - lastPowerUse[item.Key] < powerCooldown[item.Key];
            GameObject.Find(item.Value).GetComponent<Button>().interactable = !powerOnCooldown;
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
