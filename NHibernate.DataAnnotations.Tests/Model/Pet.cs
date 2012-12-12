using System.ComponentModel.DataAnnotations;

namespace NHibernate.DataAnnotations.Tests.Model
{
    public class Pet
    {
        public const string NameLengthValidation = "Cats can't remember names longer than 10 characters";
        public const string TooManyKittens = "Cat exploded";

        public virtual int Id { get; protected set; }

        [StringLength(10, ErrorMessage = NameLengthValidation)]
        public virtual string Name { get; set; }

        public virtual string Gender { get; set; }
    }
}