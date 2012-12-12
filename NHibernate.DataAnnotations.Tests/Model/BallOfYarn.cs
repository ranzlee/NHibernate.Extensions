using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHibernate.DataAnnotations.Tests.Model
{
    public class BallOfYarn : IValidatableObject
    {
        public const string NotForPlayingValidation = "The crazy cat lady will be pissed about this";
        
        public bool CascadeValidation { get { return true; } }

        public bool IsUsedForSewing { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsUsedForSewing) yield return new ValidationResult(NotForPlayingValidation);
        }
    }
}