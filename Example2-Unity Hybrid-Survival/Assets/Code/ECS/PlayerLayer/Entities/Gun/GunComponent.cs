namespace Svelto.ECS.Example.Survive.Player.Gun
{
    public struct GunComponent : IEntityComponent
    {
        public float   timeBetweenBullets;
        public float   range;
        public int     damagePerShot;
        public float   timer;
        // Ammo System
        public int     maxAmmo;
        public int     currentAmmo;
        
        public bool fired;
    }
}