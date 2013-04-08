using System;
using Glimpse.Core.Message;

namespace NHibernate.Glimpse
{
    public class NHibernateTimelineMessage : ITimelineMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public Guid Id
        {
            get { return _id; }
        }

        public TimeSpan Offset { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime StartTime { get; set; }

        public string EventName { get; set; }

        public TimelineCategoryItem EventCategory
        {
            get { return new TimelineCategoryItem("NHibernate", "#903A36", "#87802A"); } 
            set {}
        }

        public string EventSubText { get; set; }
    }
}