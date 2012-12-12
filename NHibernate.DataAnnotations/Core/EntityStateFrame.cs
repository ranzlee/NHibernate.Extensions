using NHibernate.Type;

namespace NHibernate.DataAnnotations.Core
{
    internal class EntityStateFrame
    {
        internal object Entity { get; private set; }

        internal object Id { get; private set; }

        internal PersistenceOperationEnum PersistenceOperation { get; private set; }

        internal string[] PropertyNames { get; private set; }

        internal IType[] PropertyTypes { get; private set; }

        internal object[] CurrentState { get; private set; }

        internal object[] PreviousState { get; private set; }

        internal EntityStateFrame(object entity, object id, PersistenceOperationEnum persistenceOperation, string[] propertyNames, IType[] propertyTypes, object[] currentState, object[] previousState)
        {
            Entity = entity;
            Id = id;
            PersistenceOperation = persistenceOperation;
            PropertyNames = propertyNames;
            PropertyTypes = propertyTypes;
            CurrentState = currentState;
            PreviousState = previousState;
        }
    }
}