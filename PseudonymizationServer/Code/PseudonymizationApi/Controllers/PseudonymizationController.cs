using Microsoft.AspNetCore.Mvc;

namespace PseudonymizationApi.Controllers;

[ApiController]
// [Route("[controller]")]
public class PseudonymizationController : ControllerBase
{
    private readonly ILogger<PseudonymizationController> _logger;

    public PseudonymizationController(ILogger<PseudonymizationController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("GetPseudonym")]
    public ActionResult GetPseudonym([FromBody] string? original)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("GetOriginal")]
    public ActionResult GetOriginal([FromBody] string? pseudonym)
    {
        throw new NotImplementedException();
    }
}
