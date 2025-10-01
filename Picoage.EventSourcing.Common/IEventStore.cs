namespace Picoage.EventSourcing.Common
{
    public interface IEventStore:IDisposable
    {
        Task CreateEventAsync(string id);

        Task AppendEventAsync(EventMessage eventMessage);

        Task SaveEventsAsync(); 

        Task<IEnumerable<EventMessage>> ReplyEventsAsync(string id);

    }
}
