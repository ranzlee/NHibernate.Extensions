using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Iesi.Collections.Generic;

namespace NHibernate.DataAnnotations.Tests.Model
{
    public class Cat : Pet, IValidatableObject
    {
        public const string CatsHaveNineLives = "Cats have nine lives and can't just be deleted";

        private Iesi.Collections.Generic.ISet<Cat> _kittens = new HashedSet<Cat>();
        public virtual Iesi.Collections.Generic.ISet<Cat> Kittens
        {
            get { return _kittens; }
            set { _kittens = value; }
        }
   
        public virtual Cat Parent { get; set; }

        public virtual BallOfYarn Toy { get; set; }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext.Items.GetEntityPersistenceContext().IsBeingRemoved)
            {
                yield return new ValidationResult(CatsHaveNineLives);
            }
            if (Gender == "F" && _kittens.Count > 15) yield return new ValidationResult(TooManyKittens);
            //cascade validation to toy
            if (Toy != null)
            {
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(Toy, new ValidationContext(Toy, null, null), validationResults, true);
                foreach (var validationResult in validationResults)
                {
                    yield return validationResult;
                }
            }
        }
    }
}