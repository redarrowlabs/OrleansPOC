using Common;
using GrainInterfaces;
using Orleans;
using System;

namespace Grains
{
    public abstract class BaseGrain<T> : Grain<T>
    {
        protected IEntityGrain GetEntity(Guid entityId, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Patient:
                    return GrainFactory.GetGrain<IPatientGrain>(entityId);

                case EntityType.Provider:
                    return GrainFactory.GetGrain<IProviderGrain>(entityId);

                default:
                    return null;
            }
        }
    }
}