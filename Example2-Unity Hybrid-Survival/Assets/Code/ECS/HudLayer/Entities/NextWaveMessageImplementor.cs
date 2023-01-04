using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class NextWaveMessageImplementor : MonoBehaviour, IImplementor, INextWaveMessageComponent
    {
        public bool visible
        {
            set
            {
                // TODO: Animate message
                if (value)
                {
                    _canvasGroup.alpha = 1;
                }
                else
                {
                    _canvasGroup.alpha = 0;
                }
            }
        }

        private void Awake()
        {
            // Set up the reference to the Canvas group
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private CanvasGroup _canvasGroup;
    }
}