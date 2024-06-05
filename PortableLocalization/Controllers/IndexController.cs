using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace PortableLocalization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IndexController : ControllerBase
{
    private readonly IStringLocalizer<SharedResource> _localizer;
    private readonly SharedResource _sharedResource;

    public IndexController(IStringLocalizer<SharedResource> localizer, SharedResource sharedResource)
    {
        _localizer = localizer;
        _sharedResource = sharedResource;
    }

    [HttpGet]
    public string Get()
    {
        return _sharedResource.Hello + "\r\n" +  _localizer["hello"];
    }
}
