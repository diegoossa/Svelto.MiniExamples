namespace  Svelto.ECS.MiniExamples.Doofuses.Stride
{
    public struct ChildComponent : IEntityComponent
    {
        public ChildComponent(EntityReference parent) { this.parent = parent; }

        public EntityReference parent;
    }
}