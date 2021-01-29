using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;

namespace Assets.Scripts.Systems
{
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class PickUpOnTriggerSystem : JobComponentSystem
    {
        private BuildPhysicsWorld buildPhysicsWorld;
        private StepPhysicsWorld stepPhysicsWorld;
        private EndSimulationEntityCommandBufferSystem commandBufferSystem;

        //[BurstCompile]
        private struct PickUpOnTriggerSystemJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<PickUpTag> allPickUps;
            [ReadOnly] public ComponentDataFromEntity<PlayerTag> allPlayers;
            public EntityCommandBuffer entityCommandBuffer;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;
                if (allPickUps.Exists(entityA) && allPickUps.Exists(entityB))
                {
                    return;
                }
                if (allPickUps.Exists(entityA) && allPlayers.Exists(entityB))
                {
                    entityCommandBuffer.DestroyEntity(entityA);
                    Debug.LogError(" PickUp Entity A is" + entityA + "collided with B " + entityB);
                }
                else if (allPickUps.Exists(entityB) && allPlayers.Exists(entityA))
                {
                    entityCommandBuffer.DestroyEntity(entityB);
                    Debug.LogError(" Player Entity A is" + entityA + "collided with B " + entityB);
                }
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new PickUpOnTriggerSystemJob();
            job.allPickUps = GetComponentDataFromEntity<PickUpTag>(true);
            job.allPlayers = GetComponentDataFromEntity<PlayerTag>(true);
            job.entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();

            JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

            //jobHandle.Complete();
            commandBufferSystem.AddJobHandleForProducer(jobHandle);
            return jobHandle;
        }
    }
}