namespace api.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class AgentController() : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat( string username)
        {
          
            return Ok();
        }
    }
}
