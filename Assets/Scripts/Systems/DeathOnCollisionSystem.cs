using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class DeathOnCollisionSystem : JobComponentSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    [BurstCompile]
    private struct DeathOnCollisionSystemJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<DeathColliderTag> deathColliderGroup;
        [ReadOnly] public ComponentDataFromEntity<ChaserTag> chaserGroup;
        public ComponentDataFromEntity<HealthData> healthGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool entityAisChaser = chaserGroup.Exists(entityA);
            bool entityAisDeathCollider = deathColliderGroup.Exists(entityA);
            bool entityBisChaser = chaserGroup.Exists(entityB);
            bool entityBisDeathCollider = deathColliderGroup.Exists(entityB);

            if (entityAisDeathCollider && entityBisChaser)
            {
                HealthData modifiedHealth = healthGroup[entityB];
                modifiedHealth.isDead = true;
                healthGroup[entityB] = modifiedHealth;
            }
            if (entityAisChaser && entityBisDeathCollider)
            {
                HealthData modifiedHealth = healthGroup[entityA];
                modifiedHealth.isDead = true;
                healthGroup[entityA] = modifiedHealth;
            }
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DeathOnCollisionSystemJob();
        job.deathColliderGroup = GetComponentDataFromEntity<DeathColliderTag>(true);
        job.chaserGroup = GetComponentDataFromEntity<ChaserTag>(true);
        job.healthGroup = GetComponentDataFromEntity<HealthData>(false);

        JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
        jobHandle.Complete();
        return jobHandle;
    }
}