using Election.Abstractions;
using Election.DTOs;
using Election.Entity;
using Election.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Election.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ElectionController : CustomBaseController
    {
        
        private readonly IElectionService _electionServices;

        public ElectionController(IElectionService electionServices)
        {
            _electionServices = electionServices;
        }
        [HttpPost]
        public async Task<IActionResult> GenerateCityDatas(GenerateVoterRequestMessage request)
        {
             var result = await _electionServices.GenerateVotersAsync(request);
            return CreateActionResultInstance(ResponseDTO<List<GraphResultMessageDTO>>.Success(result,200));
        }
    }
}
