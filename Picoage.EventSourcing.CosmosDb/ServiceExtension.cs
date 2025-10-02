using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Picoage.EventSourcing.Common;

namespace Picoage.EventSourcing.CosmosDb
{
    public static class ServiceExtension
    {
        public static void AddCosmosDbEventStoreWithConnectionString(this IServiceCollection serviceCollection, string connectionString, string databaseName, string containerName)
        {
            serviceCollection.AddSingleton<IEventStore>(new CosmosEventStore(connectionString, databaseName, containerName));
        }

        public static void AddCosmosDbEventStoreWithMI(this IServiceCollection serviceCollection, string endpoint, string databaseName, string containerName, string region = Regions.UKSouth)
        {
            serviceCollection.AddSingleton<IEventStore>(new CosmosEventStore(endpoint, databaseName, containerName,region));
        }
    }
}
