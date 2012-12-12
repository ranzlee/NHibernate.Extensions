using NHibernate.Cfg;
using NHibernate.DataAnnotations.Tests.Model;
using NHibernate.Session;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.DataAnnotations.Tests
{
    [TestFixture]
    public class When_using_session_auditor
    {
        private readonly Marshaler _currentScope;
        

        public When_using_session_auditor()
        {
            //configure NHibernate
            var config = new Configuration();
            config.AddClass(typeof(Cat));
            //create the database
            var tool = new SchemaExport(config);
            tool.Execute(false, true, false);
            //initialize context
            _currentScope = new Marshaler(config, typeof(ValidationInterceptor));
        }

        [Test]
        public void Can_validate_a_simple_property()
        {
            var cat = new Cat {Name = "Beetlejuice", Gender = "F"};
            try
            {
                var session = _currentScope.CurrentSession;
                session.SaveOrUpdate(cat);
                Assert.IsFalse(session.GetValidator().IsValid());
                var results = session.GetValidator().GetValidationResults();
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[cat].Count);
                Assert.AreEqual(Pet.NameLengthValidation, results[cat][0].ErrorMessage);
                session.GetValidator().Eval(session.Transaction, false);
            }
            finally
            {
                _currentScope.End();
            }
        }

        [Test]
        public void IValidatableObject__is_invoked()
        {
            var cat = new Cat { Name = "Fluffy", Gender = "F" };
            try
            {
                var session = _currentScope.CurrentSession;
                session.SaveOrUpdate(cat);
                for (var i = 0; i < 16; i++)
                {
                    var kitten = new Cat { Name = string.Format("Baby{0}", i), Gender = "F" };
                    cat.Kittens.Add(kitten);
                }
                Assert.IsFalse(session.GetValidator().IsValid());
                var results = session.GetValidator().GetValidationResults();
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[cat].Count);
                Assert.AreEqual(Pet.TooManyKittens, results[cat][0].ErrorMessage);
                session.GetValidator().Eval(session.Transaction, false);
            }
            finally
            {
                _currentScope.End();
            }
        }

        [Test]
        public void A_persistence_context_is_provided_to_IValidatableObject()
        {
            var cat = new Cat { Name = "Fluffy", Gender = "F" };
            try
            {
                var session = _currentScope.CurrentSession;
                session.SaveOrUpdate(cat);
                session.Flush();
                session.Delete(cat);
                Assert.IsFalse(session.GetValidator().IsValid());
                var results = session.GetValidator().GetValidationResults();
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[cat].Count);
                Assert.AreEqual(Cat.CatsHaveNineLives, results[cat][0].ErrorMessage);
                session.GetValidator().Eval(session.Transaction, false);
            }
            finally
            {
                _currentScope.End();
            }
        }

        [Test]
        public void Validation_can_be_cascaded_to_entity_components()
        {
            var cat = new Cat { Name = "Fluffy", Gender = "F" };
            try
            {
                var session = _currentScope.CurrentSession;
                session.SaveOrUpdate(cat);
                cat.Toy = new BallOfYarn { IsUsedForSewing = true };
                Assert.IsFalse(session.GetValidator().IsValid());
                var results = session.GetValidator().GetValidationResults();
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(1, results[cat].Count);
                Assert.AreEqual(BallOfYarn.NotForPlayingValidation, results[cat][0].ErrorMessage);
                session.GetValidator().Eval(session.Transaction, false);
            }
            finally
            {
                _currentScope.End();
            }
        }

        [Test]
        public void Can_persist_a_cat_hierarchy()
        {
            //arrange
            var grannyCat = new Cat { Name = "Granny", Gender = "F" };
            var mommyCat = new Cat { Name = "Mommy", Gender = "F" };
            var babyCat = new Cat { Name = "Baby", Gender = "F" };
            try
            {
                var session = _currentScope.CurrentSession;
                session.SaveOrUpdate(grannyCat);
                grannyCat.Kittens.Add(mommyCat);
                mommyCat.Parent = grannyCat;
                mommyCat.Kittens.Add(babyCat);
                babyCat.Parent = mommyCat;
                _currentScope.Commit();
            }
            finally
            {
                _currentScope.End();
            }
            //act
            try
            {
                var session = _currentScope.CurrentSession;
                babyCat = session.Get<Cat>(babyCat.Id);
                babyCat.Parent.Parent.Name = "Gramma";
                var siblingCat = new Cat { Name = "Sibling", Gender = "F" };
                babyCat.Parent.Kittens.Add(siblingCat);
                siblingCat.Parent = babyCat.Parent;
                _currentScope.Commit();
            }
            finally
            {
                _currentScope.End();
            }
            //assert
            try
            {
                var session = _currentScope.CurrentSession;
                babyCat = session.Get<Cat>(babyCat.Id);
                //assert persistence
                Assert.AreEqual(2, babyCat.Parent.Kittens.Count);
                Assert.AreEqual("Gramma", babyCat.Parent.Parent.Name);
            }
            finally
            {
                _currentScope.End();
            }
        }
    }
}
