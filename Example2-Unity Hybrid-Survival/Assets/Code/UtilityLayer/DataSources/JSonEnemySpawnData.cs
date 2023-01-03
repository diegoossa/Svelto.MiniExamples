using System;
using UnityEngine;

namespace Svelto.ECS.Example.Survive
{
    [Serializable]
    public class JSonEnemySpawnData
    {
        public EnemySpawnData enemySpawnData;

        public JSonEnemySpawnData(EnemySpawnData spawnData) { enemySpawnData = spawnData; }
    }

    [Serializable]
    public class EnemySpawnData
    {
        public string           enemyPrefab;
        public Vector3          spawnPoint;
        public float            spawnTime;
        public PlayerTargetType targetType;
        public int initialCount;
        public float increaseRate;
    }
}