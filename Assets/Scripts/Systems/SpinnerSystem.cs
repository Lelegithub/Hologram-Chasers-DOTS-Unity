using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public class SpinnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<SpinnerTag>().
            WithNone<PlayerTag>()
            .ForEach((ref Rotation rot, in MoveData moveData) =>
        {
            quaternion normalizedRot = math.normalize(rot.Value);
            quaternion angleToRotate = quaternion.AxisAngle(math.up(), moveData.turnSpeed * deltaTime);
            rot.Value = math.mul(normalizedRot, angleToRotate);
        }).ScheduleParallel();
    }
}