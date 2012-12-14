using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using NHibernate.Glimpse.Core;
using NHibernate.Glimpse.Extensibility;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class SqlInternalLogger : IInternalLogger
    {
        private static readonly Assembly ThisAssem = typeof(SqlInternalLogger).Assembly;
        private static readonly Assembly NhAssem = typeof(IInternalLogger).Assembly;
        private static readonly Assembly GlimpseAssem = typeof(ITab).Assembly;
        internal delegate void SqlCommandExecuted(object sender, SqlCommandExecutedArgs args);

        internal static event SqlCommandExecuted OnSqlCommandExecuted;

        public void Debug(object message)
        {
            if (message == null) return;
            if (!LoggerFactory.LogRequest()) return;
            var context = new ContextFactory().GetContextProvider().GetContext();
            if (context == null) return;
            TimerResult point = null;
            if (context.Contains("__GlimpseTimer"))
            {
                var timer = context["__GlimpseTimer"] as IExecutionTimer;
                if (timer != null)
                {
                    point = timer.Point();
                }
            }
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
                        if (Equals(assem, ThisAssem)) continue;
                        if (Equals(assem, NhAssem)) continue;
                        if (Equals(assem, GlimpseAssem)) continue;    
                    }
                    methods.Add(frame.GetMethod());
                }
            }
            var l = (IList<LogStatistic>)context[Plugin.GlimpseSqlStatsKey];
            if (l == null)
            {
                l = new List<LogStatistic>();
                context.Add(Plugin.GlimpseSqlStatsKey, l);
            }
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var frames = methods
                .Select(method => string.Format("{0} -> {1}", (method.DeclaringType == null) ? "DYNAMIC" : method.DeclaringType.ToString(), method))
                .ToList();
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            var item = new LogStatistic
                           {
                               Id = Guid.NewGuid().ToString(),
                               Sql = message.ToString(),
                               StackFrames = frames,
                               Point = point,
                               ExecutionType =
                                   (point == null)
                                       ? null
                                       : (methods.Count == 0)
                                             ? null
                                             : methods[0].DeclaringType ?? typeof (object),
                               ExecutionMethod =
                                   (MethodInfo) ((point == null) ? null : (methods.Count == 0) ? null : methods[0])
                           };
            l.Add(item);
            var d = OnSqlCommandExecuted;
            if (d != null && context.Contains("__GlimpseRequestId") && item.Point != null)
            {
                d.Invoke(this, new SqlCommandExecutedArgs
                                   {
                                       ClientId = context["__GlimpseRequestId"].ToString(),
                                       Message =
                                           new PointTimelineMessage(item.Point, item.ExecutionType, item.ExecutionMethod,
                                                                    string.Format("{0} - {1} :: {2}", item.Id, item.ExecutionType.Name, item.ExecutionMethod.Name), "ASP.NET")
                                   });
            }
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