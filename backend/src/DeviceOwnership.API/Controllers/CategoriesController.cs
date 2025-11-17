using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeviceOwnership.Core.Enums;

namespace DeviceOwnership.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController : ControllerBase
{
    /// <summary>
    /// Get all available device categories
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCategories()
    {
        var categories = Enum.GetValues<DeviceCategory>()
            .Select(c => new
            {
                value = c.ToString(),
                label = AddSpacesToCamelCase(c.ToString()),
                icon = GetCategoryIcon(c)
            })
            .ToList();

        return Ok(categories);
    }

    private string AddSpacesToCamelCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return string.Concat(text.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
    }

    private string GetCategoryIcon(DeviceCategory category)
    {
        return category switch
        {
            DeviceCategory.Smartphone => "smartphone",
            DeviceCategory.Laptop => "laptop",
            DeviceCategory.Tablet => "tablet",
            DeviceCategory.Desktop => "computer",
            DeviceCategory.Camera => "camera_alt",
            DeviceCategory.Watch => "watch",
            DeviceCategory.Headphones => "headphones",
            DeviceCategory.Speaker => "speaker",
            DeviceCategory.Television => "tv",
            DeviceCategory.GameConsole => "sports_esports",
            DeviceCategory.Drone => "flight",
            DeviceCategory.EReader => "menu_book",
            DeviceCategory.Printer => "print",
            DeviceCategory.Router => "router",
            DeviceCategory.SmartHome => "home",
            DeviceCategory.Wearable => "watch",
            DeviceCategory.Other => "devices_other",
            _ => "devices"
        };
    }
}
