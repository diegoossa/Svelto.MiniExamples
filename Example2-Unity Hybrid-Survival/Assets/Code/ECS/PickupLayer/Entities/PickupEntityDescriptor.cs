using Svelto.ECS.Example.Survive.OOPLayer;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public class PickupEntityDescriptor : 
        GenericEntityDescriptor<
            PositionComponent, 
            RotationComponent,
            PickupComponent, 
            GameObjectEntityComponent, 
            CollisionComponent, 
            VFXComponent>
    { }
}