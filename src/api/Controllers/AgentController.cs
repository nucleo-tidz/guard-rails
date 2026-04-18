namespace api.Controllers
{
    using api.RequestModels;

    using application.Services;
    using application.Services.Interfaces;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class AgentController(INucleotidzAgentService nucleotidzAgent) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message, string username, string? threadId)
        {

            var response = await nucleotidzAgent.Start(threadId, username, message);
            return Ok(response);
        }
    }
}
