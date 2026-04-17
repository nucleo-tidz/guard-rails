namespace api.Controllers
{
    using infrastructure.Agents.AzureAgents;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class AgentController(INucleotidzAgent nucleotidz) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message, string username, string? threadId)
        {
            var response = await nucleotidz.Execute(message, username, threadId);
            return Ok(response);
        }
    }
}
