namespace SpecialOffers.EventFeed
{
    public interface IEventStore
    {
        object GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
    }
}