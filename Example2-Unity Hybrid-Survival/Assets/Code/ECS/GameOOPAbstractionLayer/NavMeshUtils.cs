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
                    var targetPosition = hit.position;
                    targetPosition.y = 0.5f;
                    return targetPosition;
                }
            } 
            
            // if we didn't find the point after the 30 iterations, we just pass a random position
            return new Vector3(Random.Range(-15f, 15f), 0.5f, Random.Range(-15f, 15f));
        }
    }
}