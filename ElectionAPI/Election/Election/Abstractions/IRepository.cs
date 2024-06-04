using Election.Entity;

namespace Election.Abstractions
{
    public interface IRepository
    {
        public Task AddVoterToJson<T>(IEnumerable<T> entity, string filePath);
        public Task<IEnumerable<T>> ReadVotersFromJson<T>(string filePath);
        public Task DeleteVoterFromJson( string filePath);
        public Task AddVoterToTSV<T>(IEnumerable<T> entity, string filePath) where T : Voter;
        public Task<List<T>> ReadVotersFromTSV<T>(string filePath) where T : Voter;
        public Task DeleteVoterFromTSV( string filePath);
    }
}
