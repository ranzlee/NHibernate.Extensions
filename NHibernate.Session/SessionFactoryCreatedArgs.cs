namespace NHibernate.Session
{
    public class SessionFactoryCreatedArgs
    {
        private readonly ISessionFactory _sessionFactory;

        public SessionFactoryCreatedArgs(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
        }
    }
}