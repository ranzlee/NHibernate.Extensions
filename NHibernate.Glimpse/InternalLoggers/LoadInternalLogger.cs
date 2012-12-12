using System;
using System.Collections.Generic;
using NHibernate.Glimpse.Core;
using NHibernate.Glimpse.Providers;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class LoadInternalLogger : IInternalLogger
    {
        private const string TargetMessage = "done materializing entity";

        public void Debug(object message)
        {
            if (message == null) return;
            if (!message.ToString().ToLower().Trim().StartsWith(TargetMessage)) return;
            if (!LoggerFactory.LogRequest()) return;
            var context = new RequestContextFactory().GetRequestContextProvider().GetRequestContext();
            if (context == null) return;
            var l = (IList<LogStatistic>)context[Plugin.GlimpseSqlStatsKey];
            if (l == null)
            {
                l = new List<LogStatistic>();
                context.Add(Plugin.GlimpseSqlStatsKey, l);
            }
            l.Add(new LogStatistic
                      {
                          LoadNotification = message.ToString().Replace(TargetMessage, string.Empty).Trim().UppercaseFirst()
                      });
        }

        public void Error(object message)
        {

        }

        public void Error(object message, Exception exception)
        {

        }

        public void ErrorFormat(string format, params object[] args)
        {

        }

        public void Fatal(object message)
        {

        }

        public void Fatal(object message, Exception exception)
        {

        }

        public void Debug(object message, Exception exception)
        {

        }

        public void DebugFormat(string format, params object[] args)
        {

        }

        public void Info(object message)
        {

        }

        public void Info(object message, Exception exception)
        {

        }

        public void InfoFormat(string format, params object[] args)
        {

        }

        public void Warn(object message)
        {

        }

        public void Warn(object message, Exception exception)
        {

        }

        public void WarnFormat(string format, params object[] args)
        {

        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

        public bool IsDebugEnabled
        {
            get { return LoggerFactory.LogRequest(); }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }
    }
}