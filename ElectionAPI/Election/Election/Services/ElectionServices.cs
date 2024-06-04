using Election.Abstractions;
using Election.Concrete;
using Election.DTOs;
using Election.Entity;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Election.Services
{
    public class ElectionServices : IElectionService
    {
        private readonly IRepository _repository;
        private readonly FileSettings _fileSettings;

        public ElectionServices(IRepository repository, IOptions<FileSettings> fileSettings)
        {
            _repository = repository;
            _fileSettings = fileSettings.Value;
        }
        

        public async Task<List<GraphResultMessageDTO>> GenerateVotersAsync(GenerateVoterRequestMessage request)
        {
            await CleadDatas();
            var voters = GenerateRandomVoters(request.Population);
            await _repository.AddVoterToJson(voters, _fileSettings.JsonFilePath);
            var allDatas = await ReadFileFromJson(_fileSettings.JsonFilePath);
            List<Voter> combinedList = await SetDatas(allDatas);
            var separatedList = combinedList.GroupBy(v => v.City).ToList();
            List<GraphResultMessageDTO> resultMessage = new();
            foreach (var group in separatedList)
            {
                GraphResultMessageDTO graphResultMessageDTO = new();
                graphResultMessageDTO.CityName = group.Key;
                await SetStratigies(graphResultMessageDTO, group.ToList(), request.SampleSize, request.SampleInterval, request.SampleSizePerStratum,request.StratumType);
                resultMessage.Add(graphResultMessageDTO);
            }
            return resultMessage;
        }
        private async Task SetStratigies(GraphResultMessageDTO message , List<Voter> sampleList,int sampleSize,int sampleInterval,int sampleSizePerStratum, string stratumType)
        {
            SamplingContext samplingContext = new SamplingContext();
            samplingContext.SetStrategy(new SimpleRandomSampleStrategy());
            var randomSample = samplingContext.ExecuteStrategy(sampleList, sampleSize).ToList();
            message.SimpleSampleData = randomSample;
            samplingContext.SetStrategy(new SystematicSampleStrategy());
            var systematicSample = samplingContext.ExecuteStrategy(sampleList, sampleInterval);
            message.SystematicSampleData = systematicSample.ToList();
            samplingContext.SetStrategy(new StratifiedSampleStrategy(v => v.City));
            var stratifiedSample = samplingContext.ExecuteStrategy(sampleList, sampleSizePerStratum);
            message.StratifiedSampleData = stratifiedSample.ToList();
        }
        private async Task CleadDatas()
        {
            await _repository.DeleteVoterFromJson(_fileSettings.JsonFilePath);
            await _repository.DeleteVoterFromJson(_fileSettings.TotalTSV);
            await _repository.DeleteVoterFromTSV(_fileSettings.FirstCityTSVPath);
            await _repository.DeleteVoterFromTSV(_fileSettings.OtherCitiesJSON);
        }
        private async Task<List<Voter>> SetDatas(List<Voter> allDatas)
        {
            List<Voter> firstCityList = new();
            List<Voter> otherCityList = new();
            foreach (var voter in allDatas)
            {
                if (voter.City == "New York")
                {
                    firstCityList.Add(voter);
                }
                else
                {
                    otherCityList.Add(voter);
                }
            }
            await _repository.AddVoterToTSV(firstCityList, _fileSettings.FirstCityTSVPath);
            await _repository.AddVoterToJson(otherCityList, _fileSettings.OtherCitiesJSON);

            var combinedList = firstCityList.Concat(otherCityList).ToList();
            await _repository.AddVoterToTSV(combinedList, _fileSettings.TotalTSV);
            return combinedList;
        }
        private IEnumerable<Voter> GenerateRandomVoters(int population)
        {
            var firstNames = new List<string>
            {
                "Alice", "Bob", "Carol", "Dave", "Eve",
                "Frank", "Grace", "Heidi", "Ivan", "Judy"
            };

                        var lastNames = new List<string>
            {
                "Smith", "Johnson", "Williams", "Jones", "Brown",
                "Davis", "Miller", "Wilson", "Moore", "Taylor"
            };

            var cities = new List<string> { "New York", "Los Angeles", "Chicago" };
            var votes = new List<string> { "Candidate A", "Candidate B", "Candidate C", "Candidate D", "Candidate E" };
            var random = new Random();

            var voters = new List<Voter>();

            for (int cityIndex = 0; cityIndex < cities.Count; cityIndex++)
            {
                for (int i = 0; i < population; i++)
                {
                    var firstName = firstNames[random.Next(firstNames.Count)];
                    var lastName = lastNames[random.Next(lastNames.Count)];
                    var age = random.Next(18, 70); 
                    var city = cities[cityIndex]; 
                    var vote = votes[random.Next(votes.Count)];
                    var genders = Enum.GetValues(typeof(Gender)); // Gender enum değerlerini al
                    var randomGender = (Gender)genders.GetValue(random.Next(genders.Length));

                    voters.Add(new Voter($"{firstName} {lastName}",age,city,vote,randomGender)
                    {
                        Name = $"{firstName} {lastName}",
                        Age = age,
                        City = city,
                        Vote = vote,
                        VoterGender = randomGender
                    });
                }
            }

            return voters;
        }
        private async Task<List<Voter>> ReadFileFromJson(string filePath)
        {
            var votersFromJson = await _repository.ReadVotersFromJson<Voter>(filePath);
            return votersFromJson.ToList();
        }
        private async Task<List<Voter>> ReadFileFromTSV(string filePath)
        {
            var votersFromTsv = await _repository.ReadVotersFromTSV<Voter>(filePath);
            return votersFromTsv.ToList();
        }

    }
}
