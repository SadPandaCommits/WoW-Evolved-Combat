using Microsoft.AspNetCore.Mvc;
using WowEvolved.Models;
using WowEvolved.Services;

namespace WowEvolved.Controllers;

public class HomeController : Controller
{
    private readonly DamageAnalysisService _svc;
    public HomeController(DamageAnalysisService svc) => _svc = svc;

    private Lang GetLang() => LocalizationService.Detect(Request.Query["lang"]);

    public IActionResult Index()
    {
        ViewBag.Lang = LocalizationService.LangCode(GetLang());
        return View();
    }

    [HttpPost]
    public IActionResult Analyze(
        IFormFile? file1, IFormFile? file2,
        IFormFile? json1, IFormFile? json2,
        string? class1, string? spec1,
        string? class2, string? spec2)
    {
        var lang = GetLang();
        ViewBag.Lang = LocalizationService.LangCode(lang);

        if (file1 == null || file2 == null)
        {
            ViewBag.Error = lang == Lang.EN
                ? "Please upload two CSV files."
                : "Por favor, envie dois arquivos CSV.";
            return View("Index");
        }
        try
        {
            using var s1 = file1.OpenReadStream();
            using var s2 = file2.OpenReadStream();
            using var j1 = json1?.OpenReadStream();
            using var j2 = json2?.OpenReadStream();

            var p1 = _svc.ParseCsv(s1, file1.FileName, lang, j1, class1, spec1);
            var p2 = _svc.ParseCsv(s2, file2.FileName, lang, j2, class2, spec2);
            var result = _svc.Compare(p1, p2, lang);

            var warns = new List<string>();
            if (p1.JsonParseError != null) warns.Add($"JSON Player 1: {p1.JsonParseError}");
            if (p2.JsonParseError != null) warns.Add($"JSON Player 2: {p2.JsonParseError}");
            if (warns.Any()) ViewBag.Warnings = warns;

            return View("Result", result);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"{(lang==Lang.EN?"Error":"Erro")}: {ex.Message}";
            return View("Index");
        }
    }

    [HttpGet]
    public IActionResult GetSpecs(string className)
    {
        var lang = GetLang();
        var specs = (lang == Lang.EN
            ? WowClassRegistryEN.GetSpecsForClass(className)
            : WowClassRegistry.GetSpecsForClass(className))
            .Select(s => new { s.SpecName, s.Role }).ToList();
        return Json(specs);
    }

    [HttpGet]
    public IActionResult DownloadTemplate(string? className, string? specName)
    {
        var lang = GetLang();
        var spec = (!string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(specName))
            ? (lang == Lang.EN
                ? WowClassRegistryEN.GetSpec(className, specName)
                : WowClassRegistry.GetSpec(className, specName))
            : null;
        var json = CustomSpecProfile.GenerateTemplate(spec);
        var fn = spec != null
            ? $"wow-evolved-{spec.ClassName.ToLower().Replace(" ","-")}-{spec.SpecName.ToLower()}.json"
            : "wow-evolved-template.json";
        return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fn);
    }
}
