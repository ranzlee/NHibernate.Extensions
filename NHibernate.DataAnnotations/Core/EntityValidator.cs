using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHibernate.DataAnnotations.Core
{
    internal static class EntityValidator
    {
        internal const string ContextKey = "EntityPersistenceContext";

        internal static IEnumerable<ValidationResult> DoMemberValidation(object o, EntityPersistenceContext context)
        {
            //cascade validation through all entity components associated with the entity
            var validationResults = new List<ValidationResult>();
            //validate entity
            var validationContext = GetValidationContext(o, context);
            Validator.TryValidateObject(o,
                validationContext, 
                validationResults, 
                context.ValidateProperties);
            //validate entity components
            return validationResults;
        }

        private static ValidationContext GetValidationContext(object o, EntityPersistenceContext context)
        {
            var items = new Dictionary<object, object> { { ContextKey, context } };
            return new ValidationContext(o, null, items);
        }
    }
}