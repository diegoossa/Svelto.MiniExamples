using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class NextWaveMessageImplementor : MonoBehaviour, IImplementor, INextWaveMessageComponent
    {
        public AnimationState animationState
        {
            set => _animator.SetBool(HUDAnimations.NextWave, true);
        }

        private void Awake()
        {
            _animator = transform.parent.GetComponent<Animator>();

        }

        private Animator _animator;
    }
}