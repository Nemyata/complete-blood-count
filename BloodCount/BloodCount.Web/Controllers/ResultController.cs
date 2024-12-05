using Microsoft.AspNetCore.Mvc;

namespace BloodCount.Web.Controllers;

public class ResultController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}