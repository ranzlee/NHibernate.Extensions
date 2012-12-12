using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Event.Default;
using NHibernate.Glimpse.InternalLoggers;
using NHibernate.Transaction;

namespace NHibernate.Glimpse
{
    public class LoggerFactory : ILoggerFactory
    {
        private readonly IList<string> _loggers = new List<string>();

        public bool HasCommandLogger
        {
            get { return _loggers.Contains("command"); }
        }

        public bool HasConnectionLogger
        {
            get { return _loggers.Contains("connection"); }
        }

        public bool HasFlushLogger
        {
            get { return _loggers.Contains("flush"); }
        }

        public bool HasLoadLogger
        {
            get { return _loggers.Contains("load"); }
        }

        public bool HasTransactionLogger
        {
            get { return _loggers.Contains("transaction"); }
        }

        public LoggerFactory()
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("NHibernate.Glimpse.Loggers")) return;
            var reader = new AppSettingsReader();
            var loggersString = reader.GetValue("NHibernate.Glimpse.Loggers", typeof(string));
            if (loggersString == null) return;
            var loggers = loggersString
                .ToString()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            foreach (var logger in loggers)
            {
                _loggers.Add(logger.Trim().ToLower());
            }
        }

        public IInternalLogger LoggerFor(string keyName)
        {
            if (keyName == null) return new NoLogger();
            return keyName.ToLower().Trim() == "nhibernate.sql" ? (IInternalLogger) new SqlInternalLogger() : new NoLogger();
        }

        public IInternalLogger LoggerFor(System.Type type)
        {
            if (type == null) return new NoLogger();
            return GetLogger(type);
        }

        private IInternalLogger GetLogger(System.Type logger)
        {
            if (logger == typeof(AbstractBatcher))
            {
                if (HasCommandLogger) return new BatcherInternalLogger();
            }
            if (logger == typeof(ConnectionProvider))
            {
                if (HasConnectionLogger) return new ConnectionInternalLogger();
            }
            if (logger == typeof(DriverConnectionProvider))
            {
                if (HasConnectionLogger) return new ConnectionInternalLogger();
            }
            if (logger == typeof(AdoTransaction))
            {
                if (HasTransactionLogger) return new TransactionInternalLogger();
            }
            if (logger == typeof(TwoPhaseLoad))
            {
                if (HasLoadLogger) return new LoadInternalLogger();
            }
            if (logger == typeof(AbstractFlushingEventListener))
            {
                if (HasFlushLogger) return new FlushInternalLogger();
            }
            return new NoLogger();
        }

        internal static bool LogRequest()
        {
            return true;
        }
    }
}