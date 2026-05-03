namespace api.Controllers
{
    using api.Filter;
    using application.Services.Interfaces;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(RateLimiterFilter))]
    [ServiceFilter(typeof(ContextInitializationFilter))]
    public class AgentController(INucleotidzAgentService nucleotidzAgent, ISharedContextService sharedContextService) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message, string username, string? threadId, [FromHeader(Name ="x-client")] string clientId)
        {
            var response = await nucleotidzAgent.Start( message);
            return Ok(response);
        }
    }
}
