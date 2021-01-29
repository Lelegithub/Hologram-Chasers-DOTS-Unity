using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Assets.Scripts.Systems
{
    public class TargetToDirectionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);

            Entities.
           WithNone<PlayerTag>().
           WithAll<ChaserTag>().
           ForEach((ref MoveData moveData, ref Rotation rot, in Translation pos, in TargetData targetData) =>
           {
               if (!allTranslations.Exists(targetData.targetEntity))
               {
                   return;
               }

               Translation targetPos = allTranslations[targetData.targetEntity];
               float3 dirToTarget = targetPos.Value - pos.Value;
               moveData.direction = dirToTarget;
           }).Run();
        }
    }
}