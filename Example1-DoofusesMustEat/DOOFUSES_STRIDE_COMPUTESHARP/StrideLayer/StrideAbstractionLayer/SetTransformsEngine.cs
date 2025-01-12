using System;
using Svelto.Common;
using Svelto.Common.Internal;
using Svelto.DataStructures;

namespace Svelto.ECS.MiniExamples.Doofuses.ComputeSharp.StrideLayer
{
    /// <summary>
        ///     Iterate all the entities that have matrices and, assuming they are stride objects, set the matrices to the
        ///     matrix to the Stride Entity
        /// </summary>
        [Sequenced(nameof(StrideLayerEngineNames.SetTransformsEngine))]
        class SetTransformsEngine : IQueryingEntitiesEngine, IUpdateEngine
        {
            public SetTransformsEngine(ECSStrideEntityManager ecsStrideEntityManager)
            {
                _ECSStrideEntityManager = ecsStrideEntityManager;
            }
     
            public EntitiesDB entitiesDB { get; set; }
     
            public void Ready()        {}
     
            public string name => this.TypeName();
     
            public void Step(in float deltaTime)
            {
                if (entitiesDB.GetFilters()
                       .TryGetPersistentFilters<
                            StrideComponent>(StrideFilterContext.StrideInstanceContext, out var filters) == false)
                    return;
     
                //iterate all the filters linked to the context StrideInstanceContext
                foreach (ref var filter in filters)
                {
                    var useFilterIDAsEntityID = (uint)filter.combinedFilterID.filterID;
     
                    //the id of the filter is the id of the instancing entity.
                    var matrices = _ECSStrideEntityManager.GetInstancingTransformations(useFilterIDAsEntityID);
     
                    //each batch of instances needs to have its own array of matrices
                    //in order to allocate less often, we allocate more than needed
                    filter.ComputeFinalCount(out var entitiesCount);
                    
                    if (matrices.Length < entitiesCount)
                        Array.Resize(ref matrices, HashHelpers.Expand(entitiesCount));
                    
                    //each filter can spread over multiple groups, so we iterate the filters per group
                    int matrixIndex       = 0;
                    foreach (var (indices, currentGroup) in filter)
                    {
                        var indicesCount = indices.count;
     
                        //we get the matrices of this group
                        var (matrixComponents, _) =
                            entitiesDB.QueryEntities<MatrixComponent>(currentGroup);
     
                        //and we copy the values to the matrices array using the filters.
                        for (var i = 0; i < indicesCount; ++i)
                        {
                            matrices[matrixIndex++] = matrixComponents[indices[i]].matrix;
                        }
                    }
     
                    //finally we set the array of matrices in Stride. remember the filter id was the entityID
                    _ECSStrideEntityManager.SetInstancingTransformations(useFilterIDAsEntityID,
                        matrices, entitiesCount);
                }
            }
            readonly ECSStrideEntityManager _ECSStrideEntityManager;
        }
}