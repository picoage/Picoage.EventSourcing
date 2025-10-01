using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;


namespace Picoage.EventSourcing.Common
{
    public class EventConvertor : JsonConverter<IEvent>
    {
        public override IEvent? ReadJson(JsonReader reader, Type objectType, IEvent? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var type = jObject["$type"]?.ToString() ?? throw new JsonSerializationException("Missing $type property");

            IEvent value = CreateInstanceFromAssemble(type);
            serializer.Populate(jObject.CreateReader(), value);
            return value;
        }

        public override void WriteJson(JsonWriter writer, IEvent? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jObject = JObject.FromObject(value);
            jObject.AddFirst(new JProperty("$type", value.GetType().Name));
            jObject.WriteTo(writer);
        }

        dynamic CreateInstanceFromAssemble(string typeName)
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly()!;
            object value = new ();

            AssemblyName[] referencedAssemblies = entryAssembly.GetReferencedAssemblies();

            foreach (AssemblyName referencedAssembly in referencedAssemblies)
            {
                Assembly assembly = Assembly.Load(referencedAssembly);

                Type? typrFromAssembly = assembly.GetTypes().SingleOrDefault(e => e.Name == typeName);

                if (typrFromAssembly is null) continue;
                value = Activator.CreateInstance(typrFromAssembly) ?? throw new InvalidOperationException($"Type '{typeName}' not found");
                break;
            }
            return value;
        }
    }

}
