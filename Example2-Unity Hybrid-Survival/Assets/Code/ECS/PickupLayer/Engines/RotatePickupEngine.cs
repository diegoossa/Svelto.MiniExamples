using System.Collections;
using Svelto.ECS.Example.Survive.OOPLayer;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public class RotatePickupEngine: IQueryingEntitiesEngine, IStepEngine
    {
        private const float ROTATION_SPEED = 15f;
        
        public RotatePickupEngine(ITime time)
        {
            _time = time;
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready()
        {
            _tick = Tick();
        }
        
        public void Step() => _tick.MoveNext();

        public string name => nameof(RotatePickupEngine);

        IEnumerator Tick()
        {
            void RotatePickup()
            {
                // Have a common rotation for all the pickups
                _currentRotation += ROTATION_SPEED * _time.deltaTime;
                foreach (var ((rotations, count), _) in entitiesDB.QueryEntities<RotationComponent>(Pickup.Groups))
                {
                    for (var i = 0; i < count; i++)
                    {
                        rotations[i].rotation = Quaternion.Euler(0,_currentRotation,0);
                    }
                }
            }

            while (true)
            {
                RotatePickup();
                yield return null;
            }
        }
        
        private IEnumerator _tick;
        private float _currentRotation;
        private readonly ITime _time;
    }
}