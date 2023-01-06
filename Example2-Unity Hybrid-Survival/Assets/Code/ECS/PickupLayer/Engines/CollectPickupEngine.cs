using Svelto.Common;

namespace Svelto.ECS.Example.Survive.Pickup
{
    [Sequenced(nameof(PickupEnginesNames.CollectPickupEngine))]
    public class CollectPickupEngine: IQueryingEntitiesEngine, IStepEngine
    {
        public CollectPickupEngine()
        {

        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
            
        }

        public void Step()
        {
        }

        public string name => nameof(CollectPickupEngine);
    }
}