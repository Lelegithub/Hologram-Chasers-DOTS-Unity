﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.
            WithAny<AsteroidTag, ChaserTag>().
            WithNone<PlayerTag>().
            ForEach((ref Translation pos, in MoveData moveData, in Rotation rot) =>
        {
            float3 forwardDirection = math.forward(rot.Value);
            pos.Value += forwardDirection * moveData.speed * deltaTime;
        }).ScheduleParallel();
    }
}