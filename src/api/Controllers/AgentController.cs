namespace api.Controllers
{
    using api.Filter;

    using application.Services.Interfaces;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using model;

    [Route("api/[controller]")]
    [ApiController]


    public class AgentController(INucleotidzAgentService nucleotidzAgent, ISharedContextService sharedContextService) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]  
        [ServiceFilter(typeof(ContextInitializationFilter))]
        [ServiceFilter(typeof(TokenRateLimiter))]
        [ServiceFilter(typeof(RateLimiterFilter))]
        public async Task<IActionResult> Chat(string message, string username, string? threadId, [FromHeader(Name = "x-client")] string clientId)
        {
            var response = await nucleotidzAgent.Start(message);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> OnBoard(TokenLimitModel tokenLimitModel)
        {
            await nucleotidzAgent.OnBoard(tokenLimitModel);
            return Ok();
        }
    }
}
