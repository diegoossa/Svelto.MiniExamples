using Svelto.ECS.Example.Survive.OOPLayer;

namespace Svelto.ECS.Example.Survive.Pickup
{
    public class PickupEntityDescriptor : 
        GenericEntityDescriptor<PositionComponent, PickupComponent, GameObjectEntityComponent, CollisionComponent, VFXComponent>
    {
    }
}