using Glimpse.Core.Message;

namespace NHibernate.Glimpse.InternalLoggers
{
    internal class SqlCommandExecutedArgs
    {
        internal PointTimelineMessage Message { get; set; }
        internal string ClientId { get; set; }
    }
}