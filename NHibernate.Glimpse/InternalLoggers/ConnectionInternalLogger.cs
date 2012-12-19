using System;
using System.Globalization;
using Glimpse.Core;
using Glimpse.Core.Extensibility;
using NHibernate.Glimpse.Core;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class ConnectionInternalLogger : IInternalLogger, IPipelineInspector
    {
        private static IMessageBroker _messageBroker;
        private static Func<RuntimePolicy> _runtime;

        public void Debug(object message)
        {
            if (_runtime.Invoke() == RuntimePolicy.Off) return;
            if (message == null) return;
            if (!LoggerFactory.LogRequest()) return;
            var timestamp = DateTime.Now;
            var item = new LogStatistic(null, null)
                           {
                               ConnectionNotification =
                                   string.Format("{0}{1}", message.ToString().Trim().UppercaseFirst(),
                                                 string.Format(" @ {0}.{1}.{2}.{3}",
                                                               timestamp.Hour.ToString(CultureInfo.InvariantCulture)
                                                                        .PadLeft(2, '0'),
                                                               timestamp.Minute.ToString(CultureInfo.InvariantCulture)
                                                                        .PadLeft(2, '0'),
                                                               timestamp.Second.ToString(CultureInfo.InvariantCulture)
                                                                        .PadLeft(2, '0'),
                                                               timestamp.Millisecond.ToString(
                                                                   CultureInfo.InvariantCulture).PadLeft(3, '0')))
                           };
            Log(item);
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

        public void Setup(IPipelineInspectorContext context)
        {
            if (context == null) return;
            _runtime = context.RuntimePolicyStrategy;
            _messageBroker = context.MessageBroker;
        }

        void Log(LogStatistic logStatistic)
        {
            _messageBroker.Publish(logStatistic);
        }
    }
}