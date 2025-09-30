using Picoage.EventSourcing.Common;

namespace Picoage.EventSourcing.CosmosDb
{
    public class CosmosEvent
    {
        public string id { get; set; } = string.Empty;
        public IList<EventMessage> EventMessage { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
