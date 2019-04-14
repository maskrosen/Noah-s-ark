using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class InputSystem : ComponentSystem
{

    public struct BoatData
    {
        public readonly int Length;
        public ComponentDataArray<BoatComponent> BoatComponent;
    }


    public struct MouseDownData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<MouseButtonDownComponent> MouseButtonDown;
    }

    public struct MouseUpData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<MouseButtonUpComponent> MouseButtonUp;
    }

    public struct DragGestureData
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<DragGestureComponent> DragGesture;
    }

    [Inject]
    private MouseDownData mouseDownData;

    [Inject]
    private MouseUpData mouseUpData;

    [Inject]
    private DragGestureData dragGestureData;

    [Inject]
    private BoatData boatData; //Only to geth this to run when no input events are active

    private EntityArchetype MouseDownArchetype;
    private EntityArchetype MouseUpArchetype;
    private EntityArchetype DragGestureArchetype;

    public void Init()
    {
        MouseDownArchetype = EntityManager.CreateArchetype(typeof(MouseButtonDownComponent));
        MouseUpArchetype = EntityManager.CreateArchetype(typeof(MouseButtonUpComponent));
        DragGestureArchetype = EntityManager.CreateArchetype(typeof(DragGestureComponent));

    }

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;

        bool mouseButton0Down = false;

        for (int i = 0; i < dragGestureData.Length; i++)
        {
            PostUpdateCommands.DestroyEntity(dragGestureData.Entities[i]);
        }

        for (int i = 0; i < mouseDownData.Length; i++)
        {
            if (mouseDownData.MouseButtonDown[i].Button == 0)
                mouseButton0Down = true;
        }

        if (Input.GetMouseButtonDown(0) && !mouseButton0Down)
        {
            var mouseDown = PostUpdateCommands.CreateEntity(MouseDownArchetype);
            PostUpdateCommands.SetComponent(mouseDown, new MouseButtonDownComponent { MousePosition = Input.mousePosition, Button = 0 });
           
        }

        if (Input.GetMouseButtonUp(0))
        {

            Vector3 mousePosition = new Vector3();

            for (int i = 0; i < mouseDownData.Length; i++)
            {
                if (mouseDownData.MouseButtonDown[i].Button == 0)
                    mousePosition = mouseDownData.MouseButtonDown[i].MousePosition;

            }
            
            var dragGesture = PostUpdateCommands.CreateEntity(DragGestureArchetype);
            PostUpdateCommands.SetComponent(dragGesture, new DragGestureComponent { ClickPosition = mousePosition, Button = 0, ReleasePosition = Input.mousePosition });

            for (int i = 0; i < mouseDownData.Length; i++)
            {
                PostUpdateCommands.DestroyEntity(mouseDownData.Entities[i]);
            }

        }

    }
}
