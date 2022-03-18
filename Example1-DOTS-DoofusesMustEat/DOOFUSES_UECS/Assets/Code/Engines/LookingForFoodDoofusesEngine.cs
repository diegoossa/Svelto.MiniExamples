using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS.EntityComponents;
using Svelto.ECS.Internal;
using Svelto.ECS.Native;
using Svelto.ECS.SveltoOnDOTS;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Svelto.ECS.MiniExamples.Example1C
{
    [Sequenced(nameof(DoofusesEngineNames.LookingForFoodDoofusesEngine))]
    public class LookingForFoodDoofusesEngine : IQueryingEntitiesEngine, IJobifiedEngine
    {
        public void Ready() { }

        public LookingForFoodDoofusesEngine(IEntityFunctions nativeSwap)
        {
            _nativeDoofusesSwap = nativeSwap.ToNativeSwap<DoofusEntityDescriptor>(nameof(LookingForFoodDoofusesEngine));
            _nativeFoodSwap     = nativeSwap.ToNativeSwap<FoodEntityDescriptor>(nameof(LookingForFoodDoofusesEngine));
        }

        public string name => nameof(LookingForFoodDoofusesEngine);

        public JobHandle Execute(JobHandle _jobHandle)
        {
            //Iterate NOEATING RED doofuses to look for RED food and MOVE them to EATING state if food is found
            var handle1 = DoofusesLookingForFoodJob(_jobHandle, GameGroups.RED_FOOD_NOT_EATEN.Groups
                                                    , GameGroups.RED_DOOFUSES_NOT_EATING.Groups
                                                    , GameGroups.RED_DOOFUSES_EATING.BuildGroup
                                                    , GameGroups.RED_FOOD_EATEN.BuildGroup);

            //Iterate NOEATING BLUE doofuses to look for BLUE food and MOVE them to EATING state if food is found
            var handle2 = DoofusesLookingForFoodJob(_jobHandle, GameGroups.BLUE_FOOD_NOT_EATEN.Groups
                                                    , GameGroups.BLUE_DOOFUSES_NOT_EATING.Groups
                                                    , GameGroups.BLUE_DOOFUSES_EATING.BuildGroup
                                                    , GameGroups.BLUE_FOOD_EATEN.BuildGroup);

            //can run in parallel
            return JobHandle.CombineDependencies(handle1, handle2);
        }

        /// <summary>
        /// All the available doofuses will start to hunt for available food
        /// </summary>
        JobHandle DoofusesLookingForFoodJob
        (JobHandle inputDeps, FasterReadOnlyList<ExclusiveGroupStruct> availableFood
       , FasterReadOnlyList<ExclusiveGroupStruct> availableDoofuses, ExclusiveBuildGroup eatingDoofusesGroup
       , ExclusiveBuildGroup eatenFoodGroup)
        {
            JobHandle deps = inputDeps;

            foreach (var ((_, foodIDs, availableFoodCount), fromGoodGroup) in entitiesDB.QueryEntities<PositionEntityComponent>(
                availableFood))
            {
                foreach (var ((doofuses, doofusesIDs, doofusesCount), fromDoofusesGroup) in entitiesDB
                   .QueryEntities<MealInfoComponent>(availableDoofuses))
                {
                    var eatingDoofusesCount = math.min(availableFoodCount, doofusesCount);

                    //schedule the job
                    deps = JobHandle.CombineDependencies(deps, new LookingForFoodDoofusesJob()
                    {
                        _doofusesTargetMeals  = doofuses
                      , _doofuses             = doofusesIDs
                      , _food                 = foodIDs
                      , _nativeDoofusesSwap   = _nativeDoofusesSwap
                      , _nativeFoodSwap       = _nativeFoodSwap
                      , _doofuseEatingGroup   = eatingDoofusesGroup
                      , _eatenFoodGroup       = eatenFoodGroup
                        , _fromFoodGroup      =fromGoodGroup
                         , _fromDoofusesGroup =fromDoofusesGroup
                    }.ScheduleParallel(eatingDoofusesCount, inputDeps));
                }
            }

            return deps;
        }

        readonly NativeEntitySwap _nativeDoofusesSwap;
        readonly NativeEntitySwap _nativeFoodSwap;

        public EntitiesDB entitiesDB { private get; set; }

        [BurstCompile]
        struct LookingForFoodDoofusesJob : IJobParallelFor
        {
            [ReadOnly] public NativeEntityIDs _food;
            [ReadOnly] public NativeEntityIDs _doofuses;
            
            [WriteOnly] public NB<MealInfoComponent> _doofusesTargetMeals;
            
            public NativeEntitySwap      _nativeDoofusesSwap;
            public NativeEntitySwap      _nativeFoodSwap;
            
            public ExclusiveBuildGroup   _doofuseEatingGroup;
            public ExclusiveBuildGroup   _eatenFoodGroup;
            
            public ExclusiveGroupStruct _fromFoodGroup;
            public ExclusiveGroupStruct _fromDoofusesGroup;

#pragma warning disable 649
            /// <summary>
            /// _threadIndex will make the native entity operations thread safe
            /// </summary>
            [NativeSetThreadIndex] readonly int _threadIndex;
#pragma warning restore 649

            public void Execute(int index)
            {
                //pickup the meal for this doofus
                var mealID = new EGID(_food[(uint) index], _fromFoodGroup);
                //Set the target meal for this doofus
                _doofusesTargetMeals[index].targetMeal = new EGID(mealID.entityID, _eatenFoodGroup);

                //swap this doofus to the eating group so it won't be picked up again
               _nativeDoofusesSwap.SwapEntity(new EGID(_doofuses[index], _fromDoofusesGroup), _doofuseEatingGroup, _threadIndex);
                //swap the meal to the being eating group, so it won't be picked up again
                _nativeFoodSwap.SwapEntity(mealID, _eatenFoodGroup, _threadIndex);
            }
        }
    }
}