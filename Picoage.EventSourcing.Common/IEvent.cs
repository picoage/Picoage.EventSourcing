namespace Picoage.EventSourcing.Common
{
    public interface IEvent
    {
        string Id { get; init; }

        DateTimeOffset OccurredAt { get; init; }
    }
}
