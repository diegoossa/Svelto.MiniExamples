#if UNITY_ECS
using System;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS.Schedulers;
using Unity.Entities;
using Unity.Jobs;

namespace Svelto.ECS.SveltoOnDOTS
{
    /// <summary>
    ///     SveltoDOTS ECSEntitiesSubmissionGroup expand the _submissionScheduler responsibility to integrate the
    ///     submission of Svelto entities with the submission of DOTS ECS entities using EntityCommandBuffer.
    ///     As there is just one submissionScheduler per enginesRoot, there should be only one SveltoDOTS
    ///     ECSEntitiesSubmissionGroup
    ///     per engines group. It's expected use is showed in the class SveltoOnDOTS ECSEnginesGroup which should be used
    ///     instead of using this class directly.
    ///     Groups DOTS ECS/Svelto SystemBase engines that creates DOTS ECS entities.
    ///     Flow:
    ///     Complete all the jobs used as input dependencies (this is a sync point)
    ///     Create the new frame Command Buffer to use
    ///     Svelto entities are submitted
    ///     Svelto Add and remove callback are called
    ///     ECB is injected in all the registered engines
    ///     all the OnUpdate of the registered engines/systems are called
    ///     the DOTS ECS command buffer is flushed
    ///     all the DOTS ECS entities created that need Svelto information will be processed
    /// </summary>
    [DisableAutoCreation]
    public sealed class SveltoOnDOTSEntitiesSubmissionGroup : SystemBase, IQueryingEntitiesEngine,
        ISveltoOnDOTSSubmission
    {
        public SveltoOnDOTSEntitiesSubmissionGroup(SimpleEntitiesSubmissionScheduler submissionScheduler)
        {
            _submissionScheduler   = submissionScheduler;
            _handleLifeTimeEngines = new FasterList<HandleLifeTimeEngine>();
            _submissionEngines     = new FasterList<SveltoOnDOTSHandleCreationEngine>();
        }

        public EntitiesDB entitiesDB { get; set; }

        public void Ready()
        {
        }

        //Right, when you record a command outside of a job using the regular ECB, you don't pass it a sort key.
        //We instead use a constant for the main thread that is actually set to Int32.MaxValue. Where as the commands
        //that are recording from jobs with the ParallelWriter, get a lower value sort key from the job. Because we
        //playback the commands in order based on this sort key, the ParallelWriter commands end up happening before
        //the main thread commands. This is where your error is coming from because the Instantiate command happens at
        //the end because it's sort key is Int32.MaxValue.
        //  We don't recommend mixing the main thread and ParallelWriter commands in a single ECB for this reason.
        public void SubmitEntities(JobHandle jobHandle)
        {
            if (_submissionScheduler.paused == true)
                return;

            using (var profiler = new PlatformProfiler("SveltoDOTSEntitiesSubmissionGroup"))
            {
                using (profiler.Sample("PreSubmissionPhase"))
                {
                    PreSubmissionPhase(ref jobHandle, profiler);
                }

                //Submit Svelto Entities, calls Add/Remove/MoveTo that can be used by the IDOTS ECSSubmissionEngines
                _submissionScheduler.SubmitEntities();

                using (profiler.Sample("AfterSubmissionPhase"))
                {
                    AfterSubmissionPhase(profiler);
                }
            }
        }

        public void Add(SveltoOnDOTSHandleCreationEngine engine)
        {
            Console.LogDebug($"Add Submission Engine {engine} to the DOTS world {_ECBSystem.World.Name}");

            //this is temporary enabled because of engines that needs EntityManagers for the wrong reasons.
            _submissionEngines.Add(engine);
            engine.entityManager = _ECBSystem.EntityManager;
            engine.OnCreate();
        }

        public void Add(HandleLifeTimeEngine engine)
        {
            Console.LogDebug($"Add Submission Engine {engine} to the DOTS world {_ECBSystem.World.Name}");

            _handleLifeTimeEngines.Add(engine);
            engine.SetupQuery(EntityManager);
            engine.entitiesDB = entitiesDB;
            engine.OnCreate();
        }

        void AfterSubmissionPhase(PlatformProfiler profiler)
        {
            using (profiler.Sample("Update Submission Engines and Flush Command Buffer"))
            {
                _ECBSystem.Update();
            }

            JobHandle jobHandle = default;

            for (var i = 0; i < _handleLifeTimeEngines.count; ++i)
                jobHandle = JobHandle.CombineDependencies(jobHandle,
                    _handleLifeTimeEngines[i].ConvertPendingEntities(default));

            jobHandle.Complete();

            using (profiler.Sample("Unmark registered DOTS over Svelto entities"))
            {
                EntityManager.RemoveComponent<DOTSEntityToSetup>(_query);
            }
        }

        void PreSubmissionPhase(ref JobHandle jobHandle, PlatformProfiler profiler)
        {
            using (profiler.Sample("Complete All Pending Jobs"))
            {
                jobHandle.Complete();
            }

            foreach (var system in _submissionEngines) system.entityCommandBuffer = _ECBSystem.CreateCommandBuffer();

            foreach (var system in _handleLifeTimeEngines) system.entityCommandBuffer = _ECBSystem.CreateCommandBuffer();
        }

        [DisableAutoCreation]
        //The system is not automatically created neither automatically updated as it's not created 
        //inside neither of the groups stepped by world.Update
        class SubmissionEntitiesCommandBufferSystem : EntityCommandBufferSystem
        {
            public FasterList<SveltoOnDOTSHandleCreationEngine> submissionEngines { set; private get; }

            protected override void OnUpdate()
            {
                JobHandle combinedHandle = default;
                for (var i = 0; i < submissionEngines.count; i++)
                    combinedHandle = JobHandle.CombineDependencies(combinedHandle, submissionEngines[i].OnUpdate());

                combinedHandle.Complete();

                base.OnUpdate();
            }
        }

        protected override void OnCreate()
        {
            _ECBSystem = World.CreateSystem<SubmissionEntitiesCommandBufferSystem>();
            _ECBSystem.submissionEngines = _submissionEngines;
            _query = EntityManager.CreateEntityQuery(typeof(DOTSEntityToSetup), typeof(DOTSSveltoEGID));
        }

        protected override void OnUpdate()
        {
            throw new NotSupportedException("if this is called something broke the original design");
        }

        readonly FasterList<HandleLifeTimeEngine>             _handleLifeTimeEngines;
        readonly FasterList<SveltoOnDOTSHandleCreationEngine> _submissionEngines;

        readonly SimpleEntitiesSubmissionScheduler _submissionScheduler;
        SubmissionEntitiesCommandBufferSystem      _ECBSystem;
        EntityQuery                                _query;
    }
}
#endif