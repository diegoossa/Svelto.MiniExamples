using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS.EntityComponents;

namespace Svelto.ECS.MiniExamples.Doofuses.Stride
{
    [Sequenced(nameof(DoofusesEngineNames.VelocityToPositionDoofusesEngine))]
    public class VelocityToPositionDoofusesEngine : IQueryingEntitiesEngine, IUpdateEngine
    {
        public void Ready()
        {
        }

        public EntitiesDB entitiesDB { get; set; }

        public string name => nameof(VelocityToPositionDoofusesEngine);

        public void Step(in float deltaTime)
        {
            var doofusesEntityGroups =
                entitiesDB.QueryEntities<PositionComponent, VelocityEntityComponent, SpeedEntityComponent>(
                    GameGroups.DOOFUSES_EATING.Groups);

            foreach (var (doofuses, _) in doofusesEntityGroups)
            {
                var (buffer1, buffer2, buffer3, count) = doofuses;
                new ComputePostionFromVelocityJob((buffer1, buffer2, buffer3, count), deltaTime).Execute();
            }
        }

        readonly struct ComputePostionFromVelocityJob
        {
            public ComputePostionFromVelocityJob(
                BT<NB<PositionComponent>, NB<VelocityEntityComponent>, NB<SpeedEntityComponent>> doofuses,
                float deltaTime)
            {
                _doofuses  = doofuses;
                _deltaTime = deltaTime;
            }

            public void Execute()
            {
                for (int index = 0; index < _doofuses.count; index++)
                {
                    ref var velocity = ref _doofuses.buffer2[index].velocity;

                    var deltaPos = velocity * (_deltaTime * _doofuses.buffer3[index].speed);
                    
                    _doofuses.buffer1[index].position += deltaPos;
                }
            }

            readonly float                                                                            _deltaTime;
            readonly BT<NB<PositionComponent>, NB<VelocityEntityComponent>, NB<SpeedEntityComponent>> _doofuses;
        }
    }
}