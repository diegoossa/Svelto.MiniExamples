using System.Collections;
using System.Threading.Tasks;
using Svelto.ECS.Example.Survive.OOPLayer;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public class PickupSpawnerEngine : IQueryingEntitiesEngine, IStepEngine, IReactOnRemoveEx<PickupComponent>
    {
        const int MAX_PICKUPS = 5;

        public PickupSpawnerEngine(PickupFactory pickupFactory, INavMeshUtils navMeshUtils)
        {
            _pickupFactory = pickupFactory;
            _navMeshUtils = navMeshUtils;
            _maxPickups = MAX_PICKUPS;
        }

        public EntitiesDB entitiesDB { private get; set; }

        public void Ready()
        {
            _intervaledTick = IntervaledTick();
        }

        public void Step()
        {
            _intervaledTick.MoveNext();
        }

        public string name => nameof(PickupSpawnerEngine);

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<PickupComponent> entities,
            ExclusiveGroupStruct groupID)
        {
            if (groupID.FoundIn(Pickup.Groups))
                _maxPickups += (int) rangeOfEntities.end - (int) rangeOfEntities.start;
        }

        private IEnumerator IntervaledTick()
        {
            var pickupsToSpawnTask = PreAllocationTask();
            while (pickupsToSpawnTask.IsCompleted == false)
                yield return null;

            while (true)
            {
                if (_maxPickups > 0)
                {
                    // Pickup appears at random positions
                    var randomPosition = _navMeshUtils.RandomNavMeshPoint();
                    var task = _pickupFactory.Fetch(randomPosition);
                    while (task.GetAwaiter().IsCompleted == false)
                        yield return null;
                    _maxPickups--;
                }

                // Pickup appears at random times
                var waitForSecondsEnumerator = new WaitForSecondsEnumerator(Random.Range(4,8));
                while (waitForSecondsEnumerator.MoveNext())
                    yield return null;
            }

            async Task PreAllocationTask()
            {
                await _pickupFactory.Preallocate(MAX_PICKUPS);
            }
        }

        private readonly PickupFactory _pickupFactory;
        private INavMeshUtils _navMeshUtils;

        private int _maxPickups;
        private IEnumerator _intervaledTick;
    }
}