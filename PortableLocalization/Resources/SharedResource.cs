using Microsoft.Extensions.Localization;

namespace PortableLocalization;

public class SharedResource
{
	private readonly IStringLocalizer<SharedResource> _stringLocalizer;
    public SharedResource(IStringLocalizer<SharedResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public string Hello => _stringLocalizer["hello"];
}
