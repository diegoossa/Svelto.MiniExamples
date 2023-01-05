using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class EnemyCounterImplementor : MonoBehaviour, IImplementor, IEnemyCounterComponent
    {
        public int enemyCount
        {
            set
            {
                _enemyCount = value;
                _text.text = $"ENEMIES: {_enemyCount}";
            }
        }
        
        private void Awake()
        {
            // Set up the reference.
            _text = GetComponent<Text>();
            // Reset the enemy count.
            _enemyCount = 0;
        }

        private int _enemyCount;
        private Text _text;
    }
}