using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//distruggiamo l entity prima che alprossimo ciclo venga aggiornato il suo transform group
[UpdateBefore(typeof(TransformSystemGroup))]
public class RemoveOnDeathSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = commandBufferSystem.CreateCommandBuffer();

        Entities.
            WithAny<PlayerTag, ChaserTag>()
            .ForEach((Entity entity, in HealthData healthData) =>
        {
            if (healthData.isDead)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }).Schedule();
        //siccome questa classe è un systembase allora la jobHandle è salvata nella this.dependency
        //mentre in classi estese dal jobsystem bisogna passare la jobhandle come:
        //  JobHandle jobHandle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);

        commandBufferSystem.AddJobHandleForProducer(this.Dependency);
    }
}