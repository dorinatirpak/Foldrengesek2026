using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Services;

namespace LibrarySystem.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly DataService _data;

    public HomeController(DataService data)
    {
        _data = data;
    }

    public IActionResult Index()
    {
        return View(_data.GetDashboard());
    }
}
