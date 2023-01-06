

namespace Svelto.ECS.Example.Survive.OOPLayer
{
    /// <summary>
    /// Cameras set position directly, it should be considered an exception, as objects should be physic or path driven
    /// </summary>
    public class SyncEntitiesRotationToObjects: IQueryingEntitiesEngine, IStepEngine
    {
        public SyncEntitiesRotationToObjects(GameObjectResourceManager manager)
        {
            _manager = manager;
        }

        public void Ready() { }

        public EntitiesDB entitiesDB { get; set; }
        public void Step()
        {
            var groups = entitiesDB.FindGroups<GameObjectEntityComponent, RotationComponent>();
            //rotation only sync
            foreach (var ((entity, rotations, count), _) in entitiesDB
                            .QueryEntities<GameObjectEntityComponent, RotationComponent>(groups))
            {
                for (var i = 0; i < count; i++)
                {
                    var go = _manager[entity[i].resourceIndex];
                    var transform = go.transform;
                    transform.rotation = rotations[i].rotation;
                }
            }
        }

        public string name => nameof(SyncEntitiesRotationToObjects);

        private readonly GameObjectResourceManager _manager;
    }
}