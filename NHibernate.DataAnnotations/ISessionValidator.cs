using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace NHibernate.DataAnnotations
{
    public interface ISessionValidator
    {
        /// <summary>
        /// Runs validation on all modified entities in the session.  
        /// Commits the transaction if there are no validation errors, otherwise rollsback.
        /// </summary>
        /// <param name="transaction">The ITransaction object</param>
        /// <param name="throwException">True to throw a ValidationException if errors exist</param>
        void Eval(ITransaction transaction, bool throwException = true);

        /// <summary>
        /// Runs validation on all modified entities in the session.
        /// </summary>
        /// <returns>True, if no errors</returns>
        bool IsValid();

        /// <summary>
        /// Get a string of the concatenated error messages.
        /// </summary>
        /// <returns>string</returns>
        string GetValidationErrorString();

        /// <summary>
        /// Throw a ValidationException with a message of the validation error string.
        /// </summary>
        void ThrowValidationException();

        /// <summary>
        /// Runs validation on all modified entities in the session.
        /// </summary>
        /// <returns>Dictionary of entities with validation errors</returns>
        IDictionary<object, ReadOnlyCollection<ValidationResult>> GetValidationResults();

        /// <summary>
        /// Runs validation on all modified entities in the session.
        /// </summary>
        /// <param name="o">The entity to inspect</param>
        /// <returns>validation errors</returns>
        ReadOnlyCollection<ValidationResult> GetValidationResults(object o);
    }
}