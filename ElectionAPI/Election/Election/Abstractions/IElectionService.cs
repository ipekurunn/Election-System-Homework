using Election.DTOs;
using Election.Entity;

namespace Election.Abstractions
{
    public interface IElectionService
    {
        public Task<List<GraphResultMessageDTO>> GenerateVotersAsync(GenerateVoterRequestMessage request);
    }
}
