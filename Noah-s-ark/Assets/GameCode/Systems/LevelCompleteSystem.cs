using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteSystem : ComponentSystem
{
    public struct LevelCompleteData
    {
        public readonly int Length;
        public ComponentDataArray<LevelCompleteComponent> LevelComplete;
    }

    public Text StatusText;

    [Inject] private LevelCompleteData levelCompleteData;
    
    public void SetupGameObjects()
    {
        StatusText = GameObject.Find("GameStatusText").GetComponent<Text>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        for (int i = 0; i < levelCompleteData.Length; i++)
        {
            StatusText.text = "You got pwnd in the butt";
        }
    }
}
