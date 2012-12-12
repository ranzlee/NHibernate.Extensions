using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace NHibernate.DataAnnotations.Core
{
    internal class SessionValidator : ISessionValidator
    {
        private readonly ValidationInterceptor _validationInterceptor;

        internal SessionValidator(ValidationInterceptor validationInterceptor)
        {
            _validationInterceptor = validationInterceptor;
        }

        public void Eval(ITransaction transaction, bool throwException = true)
        {
            if (IsValid())
            {
                transaction.Commit();
                return;
            }
            transaction.Rollback();
            if (throwException) ThrowValidationException();
        }

        public bool IsValid()
        {
            return _validationInterceptor.GetValidationResults().Count == 0;
        }

        public string GetValidationErrorString()
        {
            return _validationInterceptor.ValidationErrorString;
        }

        public void ThrowValidationException()
        {
            throw new ValidationException(GetValidationErrorString());
        }

        public IDictionary<object, ReadOnlyCollection<ValidationResult>> GetValidationResults()
        {
            return _validationInterceptor.GetValidationResults();
        }

        public ReadOnlyCollection<ValidationResult> GetValidationResults(object o)
        {
            return _validationInterceptor.GetValidationResults(o);
        }
    }
}