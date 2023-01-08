using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class AmmoCounterImplementor : MonoBehaviour, IImplementor, IAmmoCounterComponent
    {
        public int currentAmmo
        {
            set
            {
                _ammoCount = value;
                _text.text = $"AMMO: {_ammoCount} / {_maxAmmo}";
            }
        }

        public int maxAmmo
        {
            set
            {
                _maxAmmo = value;
                _text.text = $"AMMO: {_ammoCount} / {_maxAmmo}";
            }
        }

        private void Awake()
        {
            // Set up the reference.
            _text = GetComponent<Text>();
        }

        private int _maxAmmo;
        private int _ammoCount;
        private Text _text;
    }
}