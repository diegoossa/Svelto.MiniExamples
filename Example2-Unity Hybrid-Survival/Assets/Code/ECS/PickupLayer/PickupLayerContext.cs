using Svelto.DataStructures;
using Svelto.ECS.Example.Survive.OOPLayer;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public static class PickupLayerContext
    {
        public static void Setup(IEntityFactory entityFactory, ITime time, INavMeshUtils navMeshUtils, FasterList<IStepEngine> orderedEngines, FasterList<IStepEngine> unorderedEngines, EnginesRoot enginesRoot, GameObjectResourceManager gameObjectResourceManager)
        {
            var pickupFactory = new PickupFactory(entityFactory, gameObjectResourceManager);
            
            var collectPickupEngine = new CollectPickupEngine();
            var rotatePickupEngine = new RotatePickupEngine(time);
            var pickupSpawnerEngine = new PickupSpawnerEngine(pickupFactory, navMeshUtils);

            //Pickup engines
            enginesRoot.AddEngine(collectPickupEngine);
            enginesRoot.AddEngine(pickupSpawnerEngine);
            enginesRoot.AddEngine(rotatePickupEngine);

            unorderedEngines.Add(pickupSpawnerEngine);
            unorderedEngines.Add(rotatePickupEngine);

            orderedEngines.Add(collectPickupEngine);
            // Add VFX Engine
        }
    }
}