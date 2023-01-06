using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Svelto.ECS.Example.Survive.Enemies
{
    public class EnemyWaveSpawnerEngine : IQueryingEntitiesEngine, IStepEngine, IReactOnRemoveEx<EnemyComponent>
    {
        private const int SECONDS_BETWEEN_WAVES = 2;
        private const int NUMBER_OF_ENEMIES_TO_PREALLOCATE = 12;

        public EnemyWaveSpawnerEngine(EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
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

        public string name => nameof(EnemyWaveSpawnerEngine);

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<EnemyComponent> entities,
            ExclusiveGroupStruct groupID)
        {
            if (groupID.FoundIn(EnemyDeadGroup.Groups))
            {
                _aliveEnemies -= (int) rangeOfEntities.end - (int) rangeOfEntities.start;
            }
        }

        IEnumerator IntervaledTick()
        {
            var enemiesToSpawnTask = PreallocationTask();
            while (enemiesToSpawnTask.IsCompleted == false)
                yield return null;

            var (enemiesToSpawn, enemyAttackData) = enemiesToSpawnTask.Result;

            while (true)
            {
                // Next Wave
                if (_aliveEnemies <= 0)
                {
                    _currentWave++;
                    
                    var waitForSecondsEnumerator = new WaitForSecondsEnumerator(SECONDS_BETWEEN_WAVES);
                    while (waitForSecondsEnumerator.MoveNext())
                        yield return null;

                    for (var i = 0; i < enemiesToSpawn.Length; i++)
                    {
                        var spawnData = enemiesToSpawn[i];
                        var spawnCount = (int) (spawnData.enemySpawnData.initialCount +
                                                spawnData.enemySpawnData.increment * (_currentWave - 1));

                        for (var j = 0; j < spawnCount; j++)
                        {
                            var task = _enemyFactory.Fetch(spawnData.enemySpawnData, enemyAttackData[i]);
                            while (task.GetAwaiter().IsCompleted == false)
                                yield return null;
                            _aliveEnemies++;
                        }
                    }
                }

                yield return null;
            }

            async Task<(JSonEnemySpawnData[] enemiestoSpawn, JSonEnemyAttackData[] enemyAttackData)> PreallocationTask()
            {
                //Data should always been retrieved through a service layer regardless the data source.
                //The benefits are numerous, including the fact that changing data source would require
                //only changing the service code. In this simple example I am not using a Service Layer
                //but you can see the point.          
                //Also note that I am loading the data only once per application run, outside the 
                //main loop. You can always exploit this pattern when you know that the data you need
                //to use will never change            
                var enemiesToSpawn = await ReadEnemySpawningDataServiceRequest();
                var enemyAttackData = await ReadEnemyAttackDataServiceRequest();

                //prebuild gameObjects to avoid spikes. For each enemy type
                for (var i = enemiesToSpawn.Length - 1; i >= 0; --i)
                {
                    var spawnData = enemiesToSpawn[i];

                    //preallocate the max number of enemies
                    await _enemyFactory.Preallocate(spawnData.enemySpawnData, NUMBER_OF_ENEMIES_TO_PREALLOCATE);
                }

                return (enemiesToSpawn, enemyAttackData);
            }
        }

        static async Task<JSonEnemySpawnData[]> ReadEnemySpawningDataServiceRequest()
        {
            var json = await Addressables.LoadAssetAsync<TextAsset>("EnemySpawningData").Task;
            var enemiesToSpawn = JsonHelper.getJsonArray<JSonEnemySpawnData>(json.text);
            return enemiesToSpawn;
        }

        static async Task<JSonEnemyAttackData[]> ReadEnemyAttackDataServiceRequest()
        {
            var json = await Addressables.LoadAssetAsync<TextAsset>("EnemyAttackData").Task;
            var enemiesAttackData = JsonHelper.getJsonArray<JSonEnemyAttackData>(json.text);
            return enemiesAttackData;
        }

        private readonly EnemyFactory _enemyFactory;

        private int _currentWave;
        private int _aliveEnemies;
        private IEnumerator _intervaledTick;
    }
}