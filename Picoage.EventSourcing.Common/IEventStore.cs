namespace Picoage.EventSourcing.Common
{
    public interface IEventStore:IDisposable
    {
        Task CreateEvent(string id);

        Task AppendEvent(EventMessage eventMessage);

        Task SaveEvents(); 

        Task<IEnumerable<EventMessage>> ReplyEvents(string id);

    }
}
