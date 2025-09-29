namespace Picoage.EventSourcing.Common
{
    public interface IEventStore
    {
        Task CreateEvent(string id);

        Task AppendEvent(EventMessage eventMessage);

        Task SaveEvents(); 

        Task<IEnumerable<EventMessage>> GetEvents(string id);

    }
}
