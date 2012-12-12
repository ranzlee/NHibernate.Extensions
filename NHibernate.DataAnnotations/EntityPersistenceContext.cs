using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.DataAnnotations
{
    public class EntityPersistenceContext
    {
        public static int Key { get { return typeof(EntityPersistenceContext).GetHashCode(); } }

        public bool ValidateProperties { get { return true; } }

        public object Id { get; internal set; }

        public string FactoryName { get; internal set; }

        public bool IsBeingAdded { get; internal set; }

        public bool IsBeingRemoved { get; internal set; }

        public bool IsBeingModified { get; internal set; }

        private IDictionary<string, IType> _types = new Dictionary<string, IType>();

        private IDictionary<string, object> _currentState = new Dictionary<string, object>();

        private IDictionary<string, object> _previousState = new Dictionary<string, object>();

        public IDictionary<string, IType> Types
        {
            get { return _types; }
            internal set { _types = value; }
        }

        public IDictionary<string, object> CurrentState 
        {
            get { return _currentState; }
            internal set { _currentState = value; }
        }

        public IDictionary<string, object> PreviousState
        {
            get { return _previousState; }
            internal set { _previousState = value; }
        }
    }
}