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

        public void Ready() { }
        
        public void Step()
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

        public string name => nameof(RotatePickupEngine);

        private float _currentRotation;
        private readonly ITime _time;
    }
}