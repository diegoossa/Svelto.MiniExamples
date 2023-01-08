using System.Collections;
using Svelto.Common;
using Svelto.ECS.Example.Survive.OOPLayer;
using Svelto.ECS.Example.Survive.Player;
using Svelto.ECS.Example.Survive.Player.Gun;

namespace Svelto.ECS.Example.Survive.Pickup
{
    [Sequenced(nameof(PickupEnginesNames.CollectPickupEngine))]
    public class CollectPickupEngine : IQueryingEntitiesEngine, IStepEngine, IReactOnRemoveEx<PickupComponent>
    {
        public CollectPickupEngine(IEntityFunctions entityFunctions, GameObjectResourceManager manager)
        {
            _entityFunctions = entityFunctions;
            _manager = manager;
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
        }

        public void Step()
        {
            foreach (var ((pickup, collision, vfx, position, entityIDs, count), currentGroup) in entitiesDB
                         .QueryEntities<PickupComponent, CollisionComponent, VFXComponent, PositionComponent>(
                             Pickup.Groups))
            {
                for (var i = 0; i < count; i++)
                {
                    ref var collisionData = ref collision[i].entityInRange;

                    // A collision was previously registered
                    if (collisionData.collides)
                    {
                        if (collisionData.otherEntityID.ToEGID(entitiesDB, out var otherEntityID))
                        {
                            if (entitiesDB.Exists<WeaponComponent>(otherEntityID))
                            {
                                var weaponComponent = entitiesDB.QueryEntity<WeaponComponent>(otherEntityID);
                                ref var gunComponent =
                                    ref entitiesDB.QueryEntity<GunComponent>(
                                        weaponComponent.weapon.ToEGID(entitiesDB));
                                gunComponent.currentAmmo += pickup[i].ammo;

                                vfx[i].vfxEvent = new VFXEvent(position[i].position);

                                // Remove Pickup Entity
                                _entityFunctions.RemoveEntity<PickupEntityDescriptor>(entityIDs[i], currentGroup);
                            }
                        }
                    }
                }
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<PickupComponent> entities,
            ExclusiveGroupStruct groupID)
        {
            var (gos, _) = entitiesDB.QueryEntities<GameObjectEntityComponent>(groupID);
            for (var i = (int) (rangeOfEntities.end - 1); i >= (int) rangeOfEntities.start; i--)
            {
                //recycle the gameObject
                _manager.Recycle(gos[i].resourceIndex, 3);
            }
        }

        public string name => nameof(CollectPickupEngine);

        private IEnumerator _checkCollision;

        private readonly IEntityFunctions _entityFunctions;
        private readonly GameObjectResourceManager _manager;
    }
}