using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glimpse.Core;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using NHibernate.Glimpse.Core;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class SqlInternalLogger : IInternalLogger, IPipelineInspector
    {
        private readonly Assembly _thisAssem = typeof(SqlInternalLogger).Assembly;
        private readonly Assembly _nhAssem = typeof(IInternalLogger).Assembly;
        private readonly Assembly _glimpseAssem = typeof (ITab).Assembly;
        
        private static IMessageBroker _messageBroker;
        private static Func<RuntimePolicy> _runtime;
        private static Func<IExecutionTimer> _timerStrategy;

        public void Debug(object message)
        {
            if (_runtime.Invoke() == RuntimePolicy.Off) return;
            if (message == null) return;
            if (!LoggerFactory.LogRequest()) return;
            var stackFrames = new System.Diagnostics.StackTrace().GetFrames();
            var methods = new List<MethodBase>();
            if (stackFrames != null)
            {
                foreach (var frame in stackFrames)
                {
                    var meth = frame.GetMethod();
                    var type = meth.DeclaringType;
                    // ReSharper disable ConditionIsAlwaysTrueOrFalse
                    //this can happen for emitted types
                    if (type != null)
                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    {
                        var assem = type.Assembly;
                        if (Equals(assem, _thisAssem)) continue;
                        if (Equals(assem, _nhAssem)) continue;
                        if (Equals(assem, _glimpseAssem)) continue;    
                    }
                    methods.Add(frame.GetMethod());
                }
            }
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var frames = methods
                .Select(method => string.Format("{0} -> {1}", (method.DeclaringType == null) ? "DYNAMIC" : method.DeclaringType.ToString(), method))
                .ToList();
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            var item = new LogStatistic(null, null)
                           {
                               Sql = message.ToString(),
                               StackFrames = frames,
                               ExecutionType = (methods.Count == 0)
                                                   ? null
                                                   : (methods[0].DeclaringType == null)
                                                         ? "Object"
                                                         : methods[0].DeclaringType.Name,
                               ExecutionMethod = (methods.Count == 0) ? null : methods[0].Name,
                           };
            SqlCommandExecuted(item);
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
            _timerStrategy = context.TimerStrategy;
            _messageBroker = context.MessageBroker;
        }

        void SqlCommandExecuted(LogStatistic logStatistic)
        {
            if (_timerStrategy == null) return;
            var timer = _timerStrategy.Invoke();
            if (timer == null) return;
            var pointTimelineMessage = new PointTimelineMessage(timer.Point(), null, null,
                                                                string.Format("{0} - {1} :: {2}", logStatistic.Id,
                                                                              logStatistic.ExecutionType,
                                                                              logStatistic.ExecutionMethod), "ASP.NET");
            _messageBroker.Publish(pointTimelineMessage);
        }

        void Log(LogStatistic logStatistic)
        {
            _messageBroker.Publish(logStatistic);
        }
    }
}