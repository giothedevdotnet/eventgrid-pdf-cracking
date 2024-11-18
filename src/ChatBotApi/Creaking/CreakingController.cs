using Azure.Messaging;

namespace ChatBotApi.Creaking;

[Route("api/[controller]")]
public class CreakingController : ControllerBase
{
    private readonly ILogger<BlobFileCreatedEventHandler> _logger;
    private readonly BlobFileCreatedEventHandler _blobFileCreatedEventHandler;

    public CreakingController(ILogger<BlobFileCreatedEventHandler> logger, BlobFileCreatedEventHandler blobFileCreatedEventHandler)
    {
        _logger = logger;
        _blobFileCreatedEventHandler = blobFileCreatedEventHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CloudEvent cloudEvent)
    {
        await _blobFileCreatedEventHandler.Handle(cloudEvent);
        return Ok();
    }
}