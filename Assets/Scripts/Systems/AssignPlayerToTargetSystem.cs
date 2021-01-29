using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Collections;

namespace Assets.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TargetToDirectionSystem))]
    public class AssignPlayerToTargetSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            AssignPLayer();
        }

        protected override void OnUpdate()
        {
            //AssignPLayer();
        }

        private void AssignPLayer()
        {
            //oppure TypeOf
            EntityQuery playerQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerTag>());
            //Entity playerEntity = playerQuery.ToEntityArray(Allocator.Temp)[0];
            Entity playerEntity = playerQuery.GetSingletonEntity();

            Entities.
                WithAll<ChaserTag>().
                ForEach((ref TargetData targetData) =>
                {
                    if (playerEntity != Entity.Null)
                    {
                        targetData.targetEntity = playerEntity;
                    }
                }).Schedule();
        }
    }
}