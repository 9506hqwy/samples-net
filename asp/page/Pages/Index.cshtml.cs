using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Page.Pages;

#pragma warning disable CA1812
internal sealed class IndexModel : PageModel
#pragma warning restore CA1812
{
    [BindProperty]
    public string? Name { get; set; }

    public IActionResult OnPost()
    {
        Console.WriteLine($"OnPost: {Name}");

        return Page();
    }
}
