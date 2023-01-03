using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class EnemyCountImplementor : MonoBehaviour, IImplementor, IEnemyCountComponent
    {
        public int enemyCount
        {
            get => _enemyCount;
            set
            {
                _enemyCount = value;
                _text.text = $"Enemies: {_enemyCount}";
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