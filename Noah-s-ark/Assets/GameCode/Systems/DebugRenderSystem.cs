using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class DebugRenderSystem : ComponentSystem
{

    public struct DebugData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Rotation;
        [ReadOnly]
        public SharedComponentDataArray<DebugRenderComponent> DebugRender;
    }

    [Inject] private DebugData debugData;

    protected override void OnUpdate()
    {

        float dt = Time.deltaTime;
        var settings = Bootstrap.Settings;
        if (!settings.debugMode)
            return;

        for (int i = 0; i < debugData.Length; i++)
        {

            var position = debugData.Position[i];
            position.Value.y = 1;
            var rotation = debugData.Rotation[i];
            var debugRender = debugData.DebugRender[i];
            
            Graphics.DrawMesh(
                debugRender.Mesh, position.Value, rotation.Value, debugRender.Material, 0
            );

        }
    }
}
