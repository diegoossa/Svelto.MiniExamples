using Svelto.DataStructures;
using Svelto.ECS.Example.Survive.Player.Ammo;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public static class PickupLayerContext
    {
        public static void Setup(FasterList<IStepEngine> orderedEngines, FasterList<IStepEngine> unorderedEngines, EnginesRoot enginesRoot)
        {
            var collectAmmoEngine = new CollectAmmoEngine();

            //Pickup engines
            enginesRoot.AddEngine(collectAmmoEngine);
        }
    }
}