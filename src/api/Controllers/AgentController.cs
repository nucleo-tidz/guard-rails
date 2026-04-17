namespace api.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using application.Services.Interfaces;
    using api.RequestModels;

    [Route("api/[controller]")]
    [ApiController]
    public class AgentController(IDocumentSeedingService documentSeedingService, INucleotidzAgent nucleotidzAgent) : ControllerBase
    {
        [HttpGet("chat/{message}/{username}/thread/{threadId}")]
        [HttpGet("chat/{message}/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Chat(string message, string username, string? threadId)
        {
            var response = await nucleotidzAgent.Start(threadId, username, message);
            return Ok(response);
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

            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { error = "File is required" });
            }

            try
            {
                using (var reader = new StreamReader(request.File.OpenReadStream()))
                {
                    string documentContent = await reader.ReadToEndAsync();

                    await documentSeedingService.SeedDocumentAsync(
                        documentContent,
                        request.File.FileName,
                        request.ChunkSize,
                        request.Overlap);
                }

                return Ok(new { message = $"Document '{request.File.FileName}' seeded successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while seeding the document", details = ex.Message });
            }
        }
    }
}
