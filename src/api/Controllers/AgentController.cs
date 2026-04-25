namespace api.Controllers
{
    using api.Filter;
    using application.Services.Interfaces;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ContextInitializationFilter))]
    public class AgentController(INucleotidzAgentService nucleotidzAgent, ISharedContextService sharedContextService) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message, string username, string? threadId)
        {
          
            var response = await nucleotidzAgent.Start( message);
            return Ok(response);
        }
    }
}
