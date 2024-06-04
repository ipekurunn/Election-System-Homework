using Election.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Election.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult CreateActionResultInstance<T>(ResponseDTO<T> response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
