using UnityEngine;
using UnityEngine.AI;

namespace Svelto.ECS.Example.Survive.OOPLayer
{
    public class NavMeshUtils : INavMeshUtils
    {
        public Vector3 RandomNavMeshPoint()
        {
            for (var i = 0; i < 30; i++)
            {
                var randomPoint = Random.insideUnitSphere * 17f;
                if (NavMesh.SamplePosition(randomPoint, out var hit, 2f, NavMesh.AllAreas))
                {
                    Debug.Log($"RANDOM POINT IN THE NAVMESH {hit.position} // {i}");
                    var targetPosition = hit.position;
                    targetPosition.y = 0.5f;
                    return targetPosition;
                }
            }

            Debug.Log($"NO HIT");
            return new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));
        }
    }
}