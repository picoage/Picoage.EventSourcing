using Picoage.EventSourcing.Common;

namespace Picoage.EventSourcing.InMemory
{
    public class EventStore : IEventStore
    {
        private readonly List<EventMessage> eventMessages = [];

        private static string Id = string.Empty;

        private static IList<InMemoryEvent>? inMemoryEvents = [];

        public Task CreateEvent(string id)
        {
            Id = id;
            return Task.CompletedTask;
        }

        public Task AppendEvent(EventMessage eventMessage)
        {
            eventMessages.Add(eventMessage);
            return Task.CompletedTask;
        }


        public Task SaveEvents()
        {
            IList<InMemoryEvent> updateInMemoryEvents = inMemoryEvents?.Where(e => e?.Id == Id).ToList()?? [];

            if (!updateInMemoryEvents.Any())
            {
                inMemoryEvents?.Add(new InMemoryEvent { Id = Id, EventMessages = eventMessages });
            }
            else
            {
                inMemoryEvents?.ToList().AddRange(updateInMemoryEvents);
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<EventMessage>> ReplyEvents(string id) => Task.Run(() => inMemoryEvents?.Where(e => e?.Id == id)
            .SelectMany(e => e?.EventMessages ?? []) ?? []);


        public void Dispose()
        {
            eventMessages.Clear();
            GC.SuppressFinalize(this);
        }
    }

    internal class InMemoryEvent
    {
        public required string Id { get; init; }

        public required IList<EventMessage> EventMessages { get; init; }
    }
}
