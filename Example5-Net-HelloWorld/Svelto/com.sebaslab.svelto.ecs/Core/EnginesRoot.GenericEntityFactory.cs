﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Svelto.Common;

namespace Svelto.ECS
{
    public partial class EnginesRoot
    {
        class GenericEntityFactory : IEntityFactory
        {
            public GenericEntityFactory(EnginesRoot weakReference)
            {
                _enginesRoot = new Svelto.DataStructures.WeakReference<EnginesRoot>(weakReference);
            }

            public EntityInitializer BuildEntity<T>
            (uint entityID, ExclusiveBuildGroup groupStructId, IEnumerable<object> implementors = null
           , [CallerMemberName] string caller = null) where T : IEntityDescriptor, new()
            {
                return _enginesRoot.Target.BuildEntity(new EGID(entityID, groupStructId)
                                                     , EntityDescriptorTemplate<T>.descriptor.componentsToBuild
                                                     , TypeCache<T>.type, implementors, caller);
            }

            public EntityInitializer BuildEntity<T>
            (EGID egid, IEnumerable<object> implementors = null
           , [CallerMemberName] string caller = null) where T : IEntityDescriptor, new()
            {
                return _enginesRoot.Target.BuildEntity(egid, EntityDescriptorTemplate<T>.descriptor.componentsToBuild
                                                     , TypeCache<T>.type, implementors, caller);
            }

            public EntityInitializer BuildEntity<T>
            (EGID egid, T entityDescriptor, IEnumerable<object> implementors
           , [CallerMemberName] string caller = null) where T : IEntityDescriptor
            {
                return _enginesRoot.Target.BuildEntity(egid, entityDescriptor.componentsToBuild, TypeCache<T>.type
                                                     , implementors, caller);
            }

            public EntityInitializer BuildEntity<T>
            (uint entityID, ExclusiveBuildGroup groupStructId, T descriptorEntity, IEnumerable<object> implementors
           , [CallerMemberName] string caller = null) where T : IEntityDescriptor
            {
                return _enginesRoot.Target.BuildEntity(new EGID(entityID, groupStructId)
                                                     , descriptorEntity.componentsToBuild, TypeCache<T>.type
                                                     , implementors, caller);
            }

            public void PreallocateEntitySpace<T>(ExclusiveGroupStruct groupStructId, uint numberOfEntities)
                where T : IEntityDescriptor, new()
            {
                _enginesRoot.Target.Preallocate(groupStructId, numberOfEntities
                                              , EntityDescriptorTemplate<T>.descriptor.componentsToBuild);
            }

            public EntityInitializer BuildEntity
            (EGID egid, IComponentBuilder[] componentsToBuild, Type type, IEnumerable<object> implementors = null
           , [CallerMemberName] string caller = null)
            {
                return _enginesRoot.Target.BuildEntity(egid, componentsToBuild, type, implementors, caller);
            }

#if UNITY_NATIVE
            public Native.NativeEntityFactory ToNative<T>
                ([CallerMemberName] string caller = null)
                where T : IEntityDescriptor, new()
            {
                return _enginesRoot.Target.ProvideNativeEntityFactoryQueue<T>(caller);
            }
#endif

            //enginesRoot is a weakreference because GenericEntityStreamConsumerFactory can be injected inside
            //engines of other enginesRoot
            readonly Svelto.DataStructures.WeakReference<EnginesRoot> _enginesRoot;
        }
    }
}