using System;

namespace ShoppingCart.EventFeed
{
    public struct Event
    {
        public long SequenceNumber { get; }
        public DateTimeOffset utcNow { get; }
        public string eventName { get; }
        public object content { get; }

        public Event(long seqNumber, DateTimeOffset utcNow, string eventName, object content)
        {
            this.SequenceNumber = seqNumber;
            this.utcNow = utcNow;
            this.eventName = eventName;
            this.content = content;
        }
    }
}