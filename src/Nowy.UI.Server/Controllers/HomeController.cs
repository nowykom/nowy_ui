using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Index = Nowy.UI.Server.Views.Home.Index;

namespace Nowy.UI.Server.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public HomeController(IWebHostEnvironment environment)
    {
        this._environment = environment;
    }

    [Route("index")]
    public IActionResult Index()
    {
        return this.View(new Index { });
    }
}
