using Picoage.EventSourcing.Common;
using Xunit;

namespace Picoage.EventSourcing.InMemory.UnitTests
{
    public class EventStoreTests
    {
        [Fact]
        public async Task Should_Create_And_Append_Events()
        {
            //Arrange
            using IEventStore eventStore = new EventStore();

            //Act
            await eventStore.CreateEvent("test-id");
            await eventStore.AppendEvent(new EventMessage
            {
                Event = new TestEvent
                {
                    Id = "event-1",
                    OccurredAt = DateTimeOffset.UtcNow,
                    Description = "Test event 1"
                },
                Headers = new Dictionary<string, object>
                {
                    { "Header1", "Value1" }
                }

            });
            await eventStore.SaveEvents();

            await eventStore.AppendEvent(new EventMessage
            {
                Event = new TestEvent
                {
                    Id = "event-2",
                    OccurredAt = DateTimeOffset.UtcNow,
                    Description = "Test event 1"
                },
                Headers = new Dictionary<string, object>
                {
                    { "Header1", "Value1" }
                }

            });
            await eventStore.SaveEvents();

            var events = await eventStore.ReplyEvents("test-id");

            //Assert
            Assert.NotNull(events);
            Assert.Equal(2, events.Count());

            Assert.IsType<TestEvent>(events.First().Event);

            Assert.Equal("event-1", events.First().Event.Id);
            Assert.Equal("event-2", events.ElementAt(1).Event.Id);

            Assert.Equal("Value1", events.First().Headers["Header1"]);
          
            Assert.Equal("Test event 1", ((TestEvent)events.First().Event).Description);


        }
    }



    public record TestEvent : IEvent
    {
        public string Id { get; init; } = string.Empty;
        public DateTimeOffset OccurredAt { get; init; }

        public string? Description { get; init; }
    }
}
