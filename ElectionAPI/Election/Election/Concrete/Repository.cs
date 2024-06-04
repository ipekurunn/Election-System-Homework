using Election.Abstractions;
using Election.Entity;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Election.Concrete
{
    public class Repository : IRepository
    {
        public async Task AddVoterToJson<T>(IEnumerable<T> entity, string filePath)
        {
            var options = new JsonSerializerOptions {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(entity, options);
             await File.WriteAllTextAsync(filePath, jsonString);
        }

        public async Task AddVoterToTSV<T>(IEnumerable<T> entities, string filePath) where T : Voter
        {
            using (var writer = new StreamWriter(filePath, append: true))
            {
                foreach (var entity in entities)
                {
                    await writer.WriteLineAsync($"{entity.Name}\t{entity.Age}\t{entity.City}\t{entity.Vote}\t{entity.VoterGender}");
                }
            }
        }


        public async Task DeleteVoterFromJson(string filePath)
        {
            await File.WriteAllTextAsync(filePath, string.Empty);
        }

        public async Task DeleteVoterFromTSV(string filePath)
        {
            File.WriteAllText(filePath, string.Empty);
        }

        public async Task<IEnumerable<T>> ReadVotersFromJson<T>(string filePath)
        {
            var jsonBytes = await File.ReadAllBytesAsync(filePath);
            using (var memoryStream = new MemoryStream(jsonBytes))
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };
                return await JsonSerializer.DeserializeAsync<IEnumerable<T>>(memoryStream, options) ?? new List<T>();
            }
        }


        public async Task<List<T>> ReadVotersFromTSV<T>(string filePath) where T : Voter
        {
            var entities = new List<T>();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var parts = line.Split('\t');
                    var entity = (T)Activator.CreateInstance(typeof(T), parts[0], int.Parse(parts[1]), parts[2], parts[3], Enum.Parse<Gender>(parts[4], ignoreCase: true));
                    entities.Add(entity);
                }
            }
            return entities;
        }


    }
}
