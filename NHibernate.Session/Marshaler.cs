using System;
using System.Web;
using NHibernate.Cfg;
using NHibernate.Context;

namespace NHibernate.Session
{
    public class Marshaler
    {
        private readonly object _lock = new object();

        private readonly Configuration _configuration;

        private readonly System.Type _sessionInterceptor;

        private readonly bool _useSingletonSession;

        private ISessionFactory _factory;

        private ISession _singletonSession;

        public event SessionFactoryCreated OnSessionFactoryCreated;

        public Marshaler(Configuration configuration, System.Type sessionInterceptor, bool useSingletonSession)
        {
            _configuration = configuration;
            _sessionInterceptor = sessionInterceptor;
            _useSingletonSession = useSingletonSession;
        }

        public Marshaler(Configuration configuration, System.Type sessionInterceptor) : this(configuration, sessionInterceptor, false) { }

        public Marshaler(Configuration configuration) : this(configuration, null) { }

        public bool HasSession
        {
            get
            {
                if (_useSingletonSession)
                {
                    lock (_lock)
                    {
                        return _factory != null && _singletonSession != null;
                    }
                }
                return _factory != null && CurrentSessionContext.HasBind(_factory);
            }
        }

        public IStatelessSession GetStatelessSession()
        {
            if (_factory != null) return _factory.OpenStatelessSession();
            if (_useSingletonSession)
            {
                lock (_lock)
                {
                    if (_factory != null) return _factory.OpenStatelessSession();
                    _factory = _configuration.BuildSessionFactory();
                    var sessionFactoryCreated = OnSessionFactoryCreated;
                    if (sessionFactoryCreated != null)
                    {
                        sessionFactoryCreated.Invoke(this, new SessionFactoryCreatedArgs(_factory));
                    }
                    return _factory.OpenStatelessSession();
                }
            }
            if (HttpContext.Current != null)
            {
                InitializeContextAwareFactory<WebSessionContext>();
            }
            else
            {
                InitializeContextAwareFactory<ThreadStaticSessionContext>();
            }
            if (_factory == null) throw new InvalidOperationException("SessionFactory was not initialized");
            return _factory.OpenStatelessSession();
        }

        public ISession CurrentSession
        {
            get
            {
                if (_useSingletonSession)
                {
                    lock (_lock)
                    {
                        if (_factory == null)
                        {
                            _factory = _configuration.BuildSessionFactory();
                            var sessionFactoryCreated = OnSessionFactoryCreated;
                            if (sessionFactoryCreated != null)
                            {
                                sessionFactoryCreated.Invoke(this, new SessionFactoryCreatedArgs(_factory));
                            }
                            return GetNewSingletonSession();
                        }
                        return _singletonSession ?? GetNewSingletonSession();
                    }
                }
                if (_factory == null)
                {
                    if (HttpContext.Current != null)
                    {
                        InitializeContextAwareFactory<WebSessionContext>();
                    }
                    else
                    {
                        InitializeContextAwareFactory<ThreadStaticSessionContext>();
                    }
                }
                if (_factory == null) throw new InvalidOperationException("SessionFactory was not initialized");
                if (CurrentSessionContext.HasBind(_factory)) return _factory.GetCurrentSession();
                var session = (_sessionInterceptor == null) ? _factory.OpenSession() : _factory.OpenSession((IInterceptor)Activator.CreateInstance(_sessionInterceptor));
                session.BeginTransaction();
                CurrentSessionContext.Bind(session);
                return session;
            }
        }

        private ISession GetNewSingletonSession()
        {
            _singletonSession = (_sessionInterceptor == null) ? _factory.OpenSession() : _factory.OpenSession((IInterceptor)Activator.CreateInstance(_sessionInterceptor));
            _singletonSession.BeginTransaction();
            return _singletonSession;
        }

        private void InitializeContextAwareFactory<T>() where T : ICurrentSessionContext
        {
            if (_factory != null) return;
            lock (_lock)
            {
                if (_factory != null) return;
                _factory = _configuration.CurrentSessionContext<T>().BuildSessionFactory();
                var sessionFactoryCreated = OnSessionFactoryCreated;
                if (sessionFactoryCreated != null)
                {
                    sessionFactoryCreated.Invoke(this, new SessionFactoryCreatedArgs(_factory));
                }
            }
        }

        public void Commit()
        {
            if (_useSingletonSession)
            {
                lock (_lock)
                {
                    if (_factory == null) return;
                    if (_singletonSession == null) return;
                    try
                    {
                        _singletonSession.Flush();
                        _singletonSession.Transaction.Commit();
                        _singletonSession.Transaction.Begin();
                    }
                    catch (Exception)
                    {
                        if (_singletonSession != null
                            && _singletonSession.Transaction != null
                            && !_singletonSession.Transaction.WasCommitted
                            && !_singletonSession.Transaction.WasRolledBack)
                        {
                            _singletonSession.Transaction.Rollback();    
                        }
                        if(_singletonSession != null && _singletonSession.IsOpen)
                        {
                            _singletonSession.Close();
                        }
                        throw;
                    }
                }
            }
            else
            {
                if (_factory == null) return;
                if (!CurrentSessionContext.HasBind(_factory)) return;
                try
                {
                    _factory.GetCurrentSession().Flush();
                    _factory.GetCurrentSession().Transaction.Commit();
                    if (HttpContext.Current == null)
                    {
                        _factory.GetCurrentSession().Transaction.Begin();
                    }
                }
                catch (Exception)
                {
                    if (_factory.GetCurrentSession() != null
                        && _factory.GetCurrentSession().Transaction != null
                        && !_factory.GetCurrentSession().Transaction.WasCommitted
                        && !_factory.GetCurrentSession().Transaction.WasRolledBack)
                    {
                        _factory.GetCurrentSession().Transaction.Rollback();    
                    }
                    var session = CurrentSessionContext.Unbind(_factory);
                    if(session != null && session.IsOpen)
                    {
                        session.Close();
                    }
                    throw;
                }
            }
        }

        public void End()
        {
            if (_useSingletonSession)
            {
                lock (_lock)
                {
                    if (_factory == null) return;
                    if (_singletonSession == null) return;
                    _singletonSession.Close();
                }
            }
            else
            {
                if (_factory == null) return;
                if (!CurrentSessionContext.HasBind(_factory)) return;
                var session = CurrentSessionContext.Unbind(_factory);
                session.Close();
            }
        }
    }
}