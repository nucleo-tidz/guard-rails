namespace api.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using infrastructure.Agents.Services;
    using api.RequestModels;

    [Route("api/[controller]")]
    [ApiController]
    public class AgentController(EmbedService embedService) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string username)
        {

            return Ok();
        }

        [HttpPost("seed-document")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SeedDocument([FromForm] SeedDocumentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            {
                string documentContent = await reader.ReadToEndAsync();

                await embedService.SeedDataAsync(
                    documentContent,
                    request.File.FileName,
                    request.CollectionName,
                    request.ChunkSize,
                    request.Overlap);
            }
            return Ok(new { message = $"Document '{request.File.FileName}' seeded successfully" });
        }
    }
}
