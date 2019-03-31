namespace Svelto.ECS.Example.Survive.Characters
{
    public struct HealthEntityStruct : IEntityStruct, INeedEGID
    {
        public int  currentHealth;
        public bool dead;

        public EGID ID { get; set; }
    }
}