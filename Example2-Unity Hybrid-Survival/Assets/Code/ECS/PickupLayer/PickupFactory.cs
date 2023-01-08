using System.Threading.Tasks;
using Svelto.DataStructures.Experimental;
using Svelto.ECS.Example.Survive.OOPLayer;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public class PickupFactory
    {
        public PickupFactory(IEntityFactory entityFactory, GameObjectResourceManager gameObjectResourceManager)
        {
            _entityFactory = entityFactory;
            _gameObjectResourceManager = gameObjectResourceManager;
        }

        public async Task Preallocate(int numberOfEnemiesToSpawn)
        {
            // TODO: Create JsonSpawnData if we have more pickup types and ids
            await _gameObjectResourceManager.Preallocate("AmmoPickup", 3, numberOfEnemiesToSpawn);
        }

        public async Task Fetch(Vector3 position)
        {
            void InitEntity(EntityReferenceHolder entityReferenceHolder, GameObject enemyGo, ValueIndex valueIndex)
            {
                EntityInitializer initializer = _entityFactory.BuildEntity<PickupEntityDescriptor>(
                    new EGID(_pickupsCreated++, Pickup.BuildGroup));

                entityReferenceHolder.reference = initializer.reference.ToULong();

                initializer.Init(
                    new PickupComponent
                    {
                        ammo = 20
                    });
                initializer.Init(
                    new GameObjectEntityComponent
                    {
                        resourceIndex = valueIndex,
                        layer = GAME_LAYERS.PICKUP_LAYER
                    });
                initializer.Init(
                    new PositionComponent
                    {
                        position = position
                    });

                enemyGo.SetActive(true);
            }

            var build = await _gameObjectResourceManager.Reuse("AmmoPickup", 3);
            ValueIndex gameObjectIndex = build;
            var pickupGO = _gameObjectResourceManager[gameObjectIndex];
            var referenceHolder = pickupGO.GetComponent<EntityReferenceHolder>();
            InitEntity(referenceHolder, pickupGO, gameObjectIndex);
        }

        private readonly IEntityFactory _entityFactory;
        private readonly GameObjectResourceManager _gameObjectResourceManager;
        private uint _pickupsCreated;
    }
}