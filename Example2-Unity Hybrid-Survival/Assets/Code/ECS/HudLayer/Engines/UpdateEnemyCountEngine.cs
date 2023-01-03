using System.Collections;
using Svelto.ECS.Example.Survive.Enemies;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class UpdateEnemyCountEngine : IQueryingEntitiesEngine, IStepEngine, IReactOnAddAndRemoveEx<EnemyComponent>
    {
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
                CheckEnemyCount();
                yield return null;
            }

            void CheckEnemyCount()
            {
                hudEntityView.enemyCountComponent.enemyCount = _enemyCount;
            }
        }


        private int _enemyCount;
        private IEnumerator _tick;
    }
}