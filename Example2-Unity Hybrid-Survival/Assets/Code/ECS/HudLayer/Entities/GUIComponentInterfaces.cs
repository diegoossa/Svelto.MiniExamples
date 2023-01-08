using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
    public interface IDamageHUDComponent
    {
        float speed      { get; }
        Color flashColor { get; }
        Color imageColor { set; get; }
        AnimationState animationState { set; }
    }

    public interface IHealthSliderComponent
    {
        int value { set; }
    }

    public interface IScoreComponent
    {
        int score { set; get; }
    }
    
    public interface IEnemyCounterComponent
    {
        int enemyCount { set; }
    }
    
    public interface INextWaveMessageComponent
    {
        bool visible { set; }
    }
    
    public interface IAmmoCounterComponent
    {
        int currentAmmo { set; }
        int maxAmmo { set; }
    }
}