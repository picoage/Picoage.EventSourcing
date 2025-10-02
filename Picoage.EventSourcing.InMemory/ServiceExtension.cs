using Microsoft.Extensions.DependencyInjection;
using Picoage.EventSourcing.Common;

namespace Picoage.EventSourcing.InMemory
{
    public static class ServiceExtension
    {
        public static void AddInMemoryEventStore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEventStore, EventStore>();
        }
    }
}
