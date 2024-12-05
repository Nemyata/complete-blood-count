using BloodCount.DomainServices.Interfaces;
using BloodCount.Web.Model;

using Microsoft.AspNetCore.Mvc;


namespace BloodCount.Web.Controllers;

public class HomeController(IAnalysisService analysisService) : Controller
{
    private readonly IAnalysisService _analysisService = analysisService;

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(FileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var result = await _analysisService.UploadFileAsync(model.File);

        return View("/Views/Result/Index.cshtml", result);
    }
}