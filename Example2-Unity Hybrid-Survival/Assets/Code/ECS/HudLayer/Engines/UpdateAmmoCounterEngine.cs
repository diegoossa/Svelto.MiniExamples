using System.Collections;
using Svelto.ECS.Example.Survive.Player.Gun;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class UpdateAmmoCounterEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public void Ready()
        {
            _tick = Tick();
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Step()
        {
            _tick.MoveNext();
        }

        public string name => nameof(UpdateAmmoCounterEngine);

        IEnumerator Tick()
        {
            while (entitiesDB.HasAny<HUDEntityViewComponent>(ECSGroups.GUICanvas) == false)
                yield return null;

            var hudEntityView = entitiesDB.QueryUniqueEntity<HUDEntityViewComponent>(ECSGroups.GUICanvas);

            while (true)
            {
                var (gun, count) = entitiesDB
                    .QueryEntities<GunComponent>(PlayerGun.Gun.Group);

                for (var i = 0; i < count; i++)
                {
                    hudEntityView.ammoCounterComponent.maxAmmo = gun[i].maxAmmo;
                    hudEntityView.ammoCounterComponent.currentAmmo = gun[i].currentAmmo;
                }
                
                yield return null;
            }
        }

        private IEnumerator _tick;
    }
}