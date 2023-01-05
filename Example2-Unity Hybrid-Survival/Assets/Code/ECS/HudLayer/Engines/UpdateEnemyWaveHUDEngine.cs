using System.Collections;
using Svelto.ECS.Example.Survive.Enemies;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class UpdateEnemyWaveHUDEngine : IQueryingEntitiesEngine, IStepEngine, IReactOnAddAndRemoveEx<EnemyComponent>
    {
        private const int SECONDS_BETWEEN_WAVES = 2;
        
        public EntitiesDB entitiesDB { set; private get; }
        public string name => nameof(EnemyMovementEngine);

        public void Ready()
        {
            _tick = Tick();
        }

        public void Step()
        {
            _tick.MoveNext();
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<EnemyComponent> entities,
            ExclusiveGroupStruct groupID)
        {
            if (groupID.FoundIn(EnemyAliveGroup.Groups))
            {
                _enemyCount += (int) rangeOfEntities.end - (int) rangeOfEntities.start;
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<EnemyComponent> entities,
            ExclusiveGroupStruct groupID)
        {
            if (groupID.FoundIn(EnemyDeadGroup.Groups))
            {
                _enemyCount -= (int) rangeOfEntities.end - (int) rangeOfEntities.start;
            }
        }

        IEnumerator Tick()
        {
            while (entitiesDB.HasAny<HUDEntityViewComponent>(ECSGroups.GUICanvas) == false)
                yield return null;

            var hudEntityView = entitiesDB.QueryUniqueEntity<HUDEntityViewComponent>(ECSGroups.GUICanvas);

            while (true)
            {
                hudEntityView.enemyCounterComponent.enemyCount = _enemyCount;

                // Prepare Next Wave
                if (_enemyCount == 0)
                {
                    // Show Next Wave Message
                    hudEntityView.nextWaveMessageComponent.visible = true;
                    
                    var waitForSecondsEnumerator = new WaitForSecondsEnumerator(SECONDS_BETWEEN_WAVES);
                    while (waitForSecondsEnumerator.MoveNext())
                        yield return null;
                    
                    // Hide Next Wave Message
                    hudEntityView.nextWaveMessageComponent.visible = false;
                }
                yield return null;
            }
        }


        private int _enemyCount;
        private IEnumerator _tick;
    }
}