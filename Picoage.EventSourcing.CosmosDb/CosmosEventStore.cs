using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Picoage.EventSourcing.Common;

namespace Picoage.EventSourcing.CosmosDb
{
    public class CosmosEventStore : IEventStore
    {

        private readonly Container container;
        private readonly CosmosClient cosmosClient;
        private CosmosClientOptions cosmosClientOptions = new();
        private IList<EventMessage> eventMessages = [];
        private static string collectionId = string.Empty;

        public CosmosEventStore(string connectionString, string databaseName, string containerName)
        {
            cosmosClientOptions.ApplicationRegion = Regions.UKSouth;
            cosmosClient = new CosmosClient(connectionString: connectionString, cosmosClientOptions);
            Database database = cosmosClient.GetDatabase(databaseName);
            database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = containerName,
                PartitionKeyPath = "/id"
            }).Wait();
            container = database.GetContainer(containerName);
        }

        public CosmosEventStore(string endpoint, string databaseName, string containerName, string region = Regions.UKSouth)
        {
            cosmosClientOptions.ApplicationRegion = region;
            cosmosClient = new CosmosClient(accountEndpoint: endpoint, tokenCredential: new DefaultAzureCredential());
            Database database = cosmosClient.GetDatabase(databaseName);
            database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = containerName,
                PartitionKeyPath = "/id"
            });
            container = database.GetContainer(containerName);
        }

        public Task CreateEvent(string id)
        {
            collectionId = id;
            return Task.CompletedTask;
        }


        public Task AppendEvent(EventMessage eventMessage)
        {
            eventMessages.Add(eventMessage);
            return Task.CompletedTask;
        }


        public void Dispose()
        {
            cosmosClient.Dispose();
            eventMessages.Clear();
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<EventMessage>> ReplyEvents(string id)
        {
            var jsonObject = await container.ReadItemAsync<JObject>(id, new PartitionKey(id));
            CosmosEvent? cosmosEvent = JsonConvert.DeserializeObject<CosmosEvent>(jsonObject.Resource.ToString());
            return cosmosEvent?.EventMessage ?? [];
        }

        public async Task SaveEvents()
        {
            CosmosEvent cosmosEvent = CreateCosmosEvent();

            JsonSerializerSettings jsonSetting = CreateJsonSerializerSettings();

            try
            {
                var jsonObject = await container.ReadItemAsync<JObject>(collectionId, new PartitionKey(collectionId));
                CosmosEvent exsistingCosmosEvent = JsonConvert.DeserializeObject<CosmosEvent>(jsonObject.Resource.ToString()) ?? new();

                cosmosEvent.EventMessage = exsistingCosmosEvent?.EventMessage?.Concat(eventMessages)?.ToList() ?? [];

                JObject jdoc = CreateJObjectFromCosmosEvent(cosmosEvent, jsonSetting);

                await container.UpsertItemAsync(jdoc, new PartitionKey(collectionId));
                eventMessages.Clear();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                JObject jdoc = CreateJObjectFromCosmosEvent(cosmosEvent, jsonSetting);

                await container.CreateItemAsync(jdoc, new PartitionKey(collectionId));
                eventMessages.Clear();
            }
        }

        private static JObject CreateJObjectFromCosmosEvent(CosmosEvent cosmosEvent, JsonSerializerSettings jsonSetting)
        {
            string json = JsonConvert.SerializeObject(cosmosEvent, jsonSetting);
            JObject jdoc = JObject.Parse(json);
            return jdoc;
        }

        private static JsonSerializerSettings CreateJsonSerializerSettings() =>

             new()
             {
                 Converters = { new EventConvertor() },
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             };


        private CosmosEvent CreateCosmosEvent() =>

             new()
             {
                 id = collectionId,
                 EventMessage = eventMessages,
                 CreatedAt = DateTimeOffset.UtcNow
             };
    }
}
