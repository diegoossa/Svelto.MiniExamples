using UnityEngine.EventSystems;

namespace Svelto.ECS.Example.Survive.Player.Ammo
{
    public class CollectAmmoEngine: IQueryingEntitiesEngine, IStepEngine
    {
        public CollectAmmoEngine()
        {

        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
            
        }

        public void Step()
        {
        }

        public string name => nameof(CollectAmmoEngine);

        
    }
}