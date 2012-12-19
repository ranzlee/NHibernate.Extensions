using System;
using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;
using Glimpse.Core.Extensibility;
using NHibernate.Impl;
using NHibernate.Glimpse.Core;
using Assist = Glimpse.Core.Tab.Assist;

namespace NHibernate.Glimpse
{
    public class Plugin : ITab, ITabSetup, IDocumentation 
    {
        private static readonly object Lock = new object();
        internal static readonly IList<ISessionFactory> SessionFactories = new List<ISessionFactory>(); 
        
        public object GetData(ITabContext context)
        {
            if (context == null) return string.Empty;
            var messages = context.GetMessages<LogStatistic>();
            if (messages == null) return string.Empty;
            var logParser = new LogParser();
            var logStatistics = messages as LogStatistic[] ?? messages.ToArray();
            var stat = logParser.Transform(logStatistics);
            if (stat == null) return string.Empty;
            var headerSection = new TabSection("Selects",
                                               "Inserts",
                                               "Updates",
                                               "Deletes",
                                               "Batch Commands");
            headerSection.AddRow()
                         .Column(stat.Selects).Strong()
                         .Column(stat.Inserts)
                         .Column(stat.Updates)
                         .Column(stat.Deletes)
                         .Column(stat.Batch)
                         .ErrorIf(stat.Selects > 50);
            var detailSection = new TabSection("Log");
            var sqlCount = 0;
            foreach (var item in logStatistics)
            {
                if (sqlCount == 50) break;
                if (!string.IsNullOrEmpty(item.CommandNotification))
                {
                    detailSection.AddRow().Column(string.Format("!<span style='color:Red;'>Command: {0}</span>!", item.CommandNotification.Trim()));
                    continue;
                }
                if (!string.IsNullOrEmpty(item.ConnectionNotification))
                {
                    detailSection.AddRow().Column(string.Format("!<span style='color:Teal;'>Connection: {0}</span>!", item.ConnectionNotification.Trim()));
                    continue;
                }
                if (!string.IsNullOrEmpty(item.FlushNotification))
                {
                    detailSection.AddRow().Column(string.Format("!<span style='color:DarkSlateGray;'>Flush: {0}</span>!", item.FlushNotification.Trim()));
                    continue;
                }
                if (!string.IsNullOrEmpty(item.LoadNotification))
                {
                    detailSection.AddRow().Column(string.Format("!<span style='color:DarkSlateBlue;'>Load: {0}</span>!", item.LoadNotification.Trim()));
                    continue;
                }
                if (!string.IsNullOrEmpty(item.TransactionNotification))
                {
                    detailSection.AddRow().Column(string.Format("!<span style='color:Darkorange;'>Transaction: {0}</span>!", item.TransactionNotification.Trim()));
                    continue;
                }
                if (!string.IsNullOrEmpty(item.Sql))
                {
                    detailSection
                        .AddRow()
                        .Column(string.Format("!<div style='color:RoyalBlue ;'>{0}</div><code class='prettyprint glimpse-code' data-codeType='sql'>{1}</code>!", item.Id, item.Sql))
                        .Selected();
                    sqlCount += 1;
                }
                var stackFrames = new TabSection("Stack Trace");
                if (item.StackFrames != null && item.StackFrames.Count > 0)
                {
                    foreach (var stackFrame in item.StackFrames)
                    {
                        stackFrames.AddRow().Column(stackFrame);
                    }
                    detailSection.AddRow().Column(stackFrames);
                }
            }
            var data = Assist.Plugin.Create("Section", "Content");
            data.AddRow().Column("Request Summary").Column(headerSection);
            data.AddRow().Column("Request Details").Column(detailSection);
            if (!SessionFactories.Any(f => f.Statistics.IsStatisticsEnabled)) return data;
            foreach (var sessionFactory in SessionFactories)
            {
                var factoryDetailSection = new TabSection("Statistic", "Value");
                factoryDetailSection.AddRow().Column("Close Statement Count").Column(sessionFactory.Statistics.CloseStatementCount);
                factoryDetailSection.AddRow().Column("Collection Fetch Count").Column(sessionFactory.Statistics.CollectionFetchCount);
                factoryDetailSection.AddRow().Column("Collection Load Count").Column(sessionFactory.Statistics.CollectionLoadCount);
                factoryDetailSection.AddRow().Column("Collection Recreate Count").Column(sessionFactory.Statistics.CollectionRecreateCount);
                factoryDetailSection.AddRow().Column("Collection Remove Count").Column(sessionFactory.Statistics.CollectionRemoveCount);
                factoryDetailSection.AddRow().Column("Collection Role Names").Column(sessionFactory.Statistics.CollectionRoleNames);
                factoryDetailSection.AddRow().Column("Collection Update Count").Column(sessionFactory.Statistics.CollectionUpdateCount);
                factoryDetailSection.AddRow().Column("Connect Count").Column(sessionFactory.Statistics.ConnectCount);
                factoryDetailSection.AddRow().Column("Entity Delete Count").Column(sessionFactory.Statistics.EntityDeleteCount);
                factoryDetailSection.AddRow().Column("Entity Fetch Count").Column(sessionFactory.Statistics.EntityFetchCount);
                factoryDetailSection.AddRow().Column("Entity Insert Count").Column(sessionFactory.Statistics.EntityInsertCount);
                factoryDetailSection.AddRow().Column("Entity Load Count").Column(sessionFactory.Statistics.EntityLoadCount);
                factoryDetailSection.AddRow().Column("Entity Names").Column(sessionFactory.Statistics.EntityNames);
                factoryDetailSection.AddRow().Column("Entity Update Count").Column(sessionFactory.Statistics.EntityUpdateCount);
                factoryDetailSection.AddRow().Column("Flush Count").Column(sessionFactory.Statistics.FlushCount);
                factoryDetailSection.AddRow().Column("Optimistic Failure Count").Column(sessionFactory.Statistics.OptimisticFailureCount);
                factoryDetailSection.AddRow().Column("Prepare Statement Count").Column(sessionFactory.Statistics.PrepareStatementCount);
                factoryDetailSection.AddRow().Column("Queries").Column(sessionFactory.Statistics.Queries);
                factoryDetailSection.AddRow().Column("Query Cache Hit Count").Column(sessionFactory.Statistics.QueryCacheHitCount);
                factoryDetailSection.AddRow().Column("Query Cache Miss Count").Column(sessionFactory.Statistics.QueryCacheMissCount);
                factoryDetailSection.AddRow().Column("Query Cache Put Count").Column(sessionFactory.Statistics.QueryCachePutCount);
                factoryDetailSection.AddRow().Column("Query Execution Count").Column(sessionFactory.Statistics.QueryExecutionCount);
                factoryDetailSection.AddRow().Column("Query Execution Max Time").Column(sessionFactory.Statistics.QueryExecutionMaxTime);
                factoryDetailSection.AddRow().Column("Query Execution Max Time QueryString").Column(sessionFactory.Statistics.QueryExecutionMaxTimeQueryString);
                factoryDetailSection.AddRow().Column("Second Level Cache Hit Count").Column(sessionFactory.Statistics.SecondLevelCacheHitCount);
                factoryDetailSection.AddRow().Column("Second Level Cache Miss Count").Column(sessionFactory.Statistics.SecondLevelCacheMissCount);
                factoryDetailSection.AddRow().Column("Second Level Cache Put Count").Column(sessionFactory.Statistics.SecondLevelCachePutCount);
                factoryDetailSection.AddRow().Column("Second Level Cache Region Names").Column(sessionFactory.Statistics.SecondLevelCacheRegionNames);
                factoryDetailSection.AddRow().Column("Session Close Count").Column(sessionFactory.Statistics.SessionCloseCount);
                factoryDetailSection.AddRow().Column("Session Open Count").Column(sessionFactory.Statistics.SessionOpenCount);
                factoryDetailSection.AddRow().Column("Start Time").Column(sessionFactory.Statistics.StartTime);
                factoryDetailSection.AddRow().Column("Successful Transaction Count").Column(sessionFactory.Statistics.SuccessfulTransactionCount);
                factoryDetailSection.AddRow().Column("Transaction Count").Column(sessionFactory.Statistics.TransactionCount);
                var impl = sessionFactory as SessionFactoryImpl;
                data.AddRow().Column(string.Format("Factory: {0}", (impl == null) ? string.Empty : impl.Uuid)).Column(factoryDetailSection);
            }
            return data;
        }

        public string Name
        {
            get { return "NHibernate"; }
        }

        public RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.EndRequest; } 
        }

        public System.Type RequestContextType
        {
            get { return null; }
        }

        /// <summary>
        /// Register an ISessionFactory for statistics logging
        /// </summary>
        /// <param name="sessionFactory">ISessionFactory</param>
        public static void RegisterSessionFactory(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new NullReferenceException("sessionFactory");
            lock (Lock)
            {
                if (!SessionFactories.Contains(sessionFactory)) SessionFactories.Add(sessionFactory);    
            }
        }

        public string DocumentationUri
        {
            get { return "https://github.com/ranzlee/NHibernate.Extensions/wiki/NHibernate.Glimpse"; }
        }

        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<LogStatistic>();
        }
    }
}