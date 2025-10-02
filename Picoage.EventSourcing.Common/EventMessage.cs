namespace Picoage.EventSourcing.Common
{
    public class EventMessage
    {
        public required IEvent Event { get; init; }

        public IDictionary<string, object> Headers { get; init; } = new Dictionary<string, object>();

        public string EventName => Event.GetType().Name;

    }
}
