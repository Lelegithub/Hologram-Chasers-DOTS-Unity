//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;

//public class FaceDirectionSystem : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        Entities.
//            WithAll<PlayerTag>().
//            ForEach((ref Rotation rot, in Translation pos, in MoveData moveData) =>
//            {
//                FaceDirection(ref rot, moveData);
//            }).Schedule();

//        ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);
//        Entities.
//            WithNone<PlayerTag>().
//            WithAll<ChaserTag>()

//            .ForEach((ref MoveData moveData, ref Rotation rot, in Translation pos, in TargetData targetData) =>
//            {
//                if (!allTranslations.Exists(targetData.targetEntity))
//                {
//                    return;
//                }
//                Translation targetPos = allTranslations[targetData.targetEntity];
//                float3 dirToTarget = targetPos.Value - pos.Value;
//                moveData.direction = dirToTarget;
//                FaceDirection(ref rot, moveData);
//            }).Run();
//    }

//    private static void FaceDirection(ref Rotation rot, MoveData moveData)
//    {
//        if (!moveData.direction.Equals(float3.zero))
//        {
//            quaternion targetRotation = quaternion.LookRotationSafe(moveData.direction, math.up());
//            rot.Value = math.slerp(rot.Value, targetRotation, moveData.turnSpeed);
//        }
//    }
//}

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
public class FaceDirectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.
            ForEach((ref Rotation rot, in Translation pos, in MoveData moveData) =>
            {
                FaceDirection(ref rot, moveData);
            }).Schedule();
    }

    private static void FaceDirection(ref Rotation rot, MoveData moveData)
    {
        if (!moveData.direction.Equals(float3.zero))
        {
            quaternion targetRotation = quaternion.LookRotationSafe(moveData.direction, math.up());
            rot.Value = math.slerp(rot.Value, targetRotation, moveData.turnSpeed);
        }
    }
}