using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class AmmoCounterImplementor : MonoBehaviour, IImplementor, IAmmoCounterComponent
    {
        public int value
        {
            set
            {
                _ammoCount = value;
                _text.text = $"AMMO: {_ammoCount}";
            }
        }
        
        private void Awake()
        {
            // Set up the reference.
            _text = GetComponent<Text>();
            // Reset the ammo count.
            _ammoCount = 0;
        }

        private int _ammoCount;
        private Text _text;
    }
}