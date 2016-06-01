using Newtonsoft.Json;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Storage
{
    public class JsonStorage : IStorageProvider
    {
        private string _baseFilePath;

        public Logger Log { get; private set; }

        public string Name { get; private set; }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            Name = name;
            Log = providerRuntime.GetLogger($"StorageProvider.Json.{providerRuntime.ServiceId}");

            var codeBaseUri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            var filePath = Path.GetDirectoryName(Uri.UnescapeDataString(codeBaseUri.Path));
            _baseFilePath = Path.Combine(filePath, "Data");

            if (!Directory.Exists(_baseFilePath))
            {
                Directory.CreateDirectory(_baseFilePath);
            }

            return TaskDone.Done;
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var key = grainReference.GetPrimaryKey();
            var filePath = Path.Combine(_baseFilePath, $"{grainType}-{key}.json");
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(Path.Combine(_baseFilePath, $"{key}.json"));
                grainState.State = JsonConvert.DeserializeObject(json, grainState.State.GetType());
            }

            grainState.ETag = Guid.NewGuid().ToString();

            return TaskDone.Done;
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var key = grainReference.GetPrimaryKey();
            var json = JsonConvert.SerializeObject(grainState.State, Formatting.Indented);
            File.WriteAllText(Path.Combine(_baseFilePath, $"{grainType}-{key}.json"), json);

            return TaskDone.Done;
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var key = grainReference.GetPrimaryKey();
            File.Delete(Path.Combine(_baseFilePath, $"{grainType}-{key}.json"));

            return TaskDone.Done;
        }

        public Task Close()
        {
            return TaskDone.Done;
        }
    }
}