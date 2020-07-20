using System.Collections.Generic;

namespace ShoppingCart.EventFeed
{
    public interface IEventStore
    {
        void Raise(string v, object p);
        IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
    }
}