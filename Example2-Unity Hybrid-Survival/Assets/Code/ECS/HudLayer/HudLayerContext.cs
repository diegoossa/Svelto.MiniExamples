using Svelto.DataStructures;

namespace Svelto.ECS.Example.Survive.HUD
{
    public static class HudLayerContext
    {
        /// <summary>
        /// HudLayer is the only layer in this example that will interface with gameobject using the Svelto Implementors
        /// technique. Which is fine for simple GUI like ths one.
        /// </summary>
        public static void Setup(FasterList<IStepEngine> orderedEngines, EnginesRoot enginesRoot)
        {
            //hud engines
            var hudEngine = new HUDEngine();
            var scoreEngine = new UpdateScoreEngine();
            var enemyCountEngine = new UpdateEnemyCountEngine();
            var restartGameOnPlayerDeath = new RestartGameOnPlayerDeathEngine();

            enginesRoot.AddEngine(hudEngine);
            enginesRoot.AddEngine(scoreEngine);
            enginesRoot.AddEngine(enemyCountEngine);
            enginesRoot.AddEngine(restartGameOnPlayerDeath);

            var unsortedDamageEngines = new HUDEngines(
                new FasterList<IStepEngine>(hudEngine, scoreEngine, enemyCountEngine, restartGameOnPlayerDeath));
            
            orderedEngines.Add(unsortedDamageEngines);
        }
    }
}