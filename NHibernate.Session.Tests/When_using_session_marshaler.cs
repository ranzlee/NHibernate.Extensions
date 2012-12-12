using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Session.Tests
{
    [TestFixture]
    public class When_using_session_marshaler
    {
        [Test]
        public void Marshaler_creates_factory_and_session()
        {
            //configure NHibernate
            var config = new Configuration();
            //initialize context
            var session = new Marshaler(config).CurrentSession;
            Assert.IsNotNull(session);
            session.Dispose();
        }

        [Test]
        public void Marshaler_returns_same_session_in_thread_static_context()
        {
            //configure NHibernate
            var config = new Configuration();
            //initialize context
            var session1 = new Marshaler(config).CurrentSession;
            Assert.IsNotNull(session1);
            var session2 = new Marshaler(config).CurrentSession;
            Assert.AreSame(session1, session2);
            session1.Dispose();
            session2.Dispose();
        }

        [Test]
        public void Marshaler_can_access_stateless_session_before_stateful_session()
        {
            //configure NHibernate
            var config = new Configuration();
            //initialize context
            var session1 = new Marshaler(config).GetStatelessSession();
            Assert.IsNotNull(session1);
            var session2 = new Marshaler(config).CurrentSession;
            Assert.IsNotNull(session2);
            Assert.AreNotSame(session1, session2);
            session1.Dispose();
            session2.Dispose();
        }

        [Test]
        public void Marshaler_can_access_stateful_session_before_stateless_session()
        {
            //configure NHibernate
            var config = new Configuration();
            //initialize context
            var session1 = new Marshaler(config).CurrentSession;
            Assert.IsNotNull(session1);
            var session2 = new Marshaler(config).GetStatelessSession();
            Assert.IsNotNull(session2);
            Assert.AreNotSame(session1, session2);
            session1.Dispose();
            session2.Dispose();
        }

        [Test]
        public void Marshaler_returns_new_stateless_sessions()
        {
            //configure NHibernate
            var config = new Configuration();
            //initialize context
            var session1 = new Marshaler(config).GetStatelessSession();
            Assert.IsNotNull(session1);
            var session2 = new Marshaler(config).GetStatelessSession();
            Assert.IsNotNull(session2);
            Assert.AreNotSame(session1, session2);
            session1.Dispose();
            session2.Dispose();
        }
    }
}
