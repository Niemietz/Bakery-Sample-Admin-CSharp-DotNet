using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using BakerySample.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BakerySample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var homeModel = new HomeViewModel();
        
        using (StreamReader r = new StreamReader("data/siteData.json"))
        {
            string json = r.ReadToEnd();
            homeModel = JsonConvert.DeserializeObject<HomeViewModel>(json);
        }

        return View(homeModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost("/")]
    public IActionResult FormSubmit([FromForm] EditFormModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var url = Url.Action("", "", new { success = true });

        var newSiteData = new HomeViewModel();
        
        newSiteData.Hero = new HeroModel { Title = model.heroTitle, Subtitle =  model.heroSubtitle };

        var newSiteDataJson = JsonConvert.SerializeObject(newSiteData);
        var newSiteDataJsonObj = JsonConvert.DeserializeObject<JObject>(newSiteDataJson);

        var siteDataJsonFile = Path.Combine("data", "siteData.json");
        using (var r = new StreamReader(siteDataJsonFile))
        {
            var currentSiteDataJson = r.ReadToEnd();
            currentSiteDataJson = CleanJson(currentSiteDataJson);

            var currentSiteDataJsonObj = JsonConvert.DeserializeObject<JObject>(currentSiteDataJson);
            currentSiteDataJsonObj.Merge(newSiteDataJsonObj, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            var mergedSiteDataJson = JsonConvert.SerializeObject(currentSiteDataJson, Formatting.Indented);
            mergedSiteDataJson = CleanJson(mergedSiteDataJson, true);
            
            r.Close();

            System.IO.File.WriteAllText(siteDataJsonFile, mergedSiteDataJson);
        }

        // Perform the redirect
        return Redirect(url);
    }

    static string CleanJson(string json, bool writingToJsonFile = false)
    {
        // Trim whitespace
        json = json.Trim();

        // Normalize quotes (optional, but it ensures consistency)
        if (!writingToJsonFile)
        {
            json = json.Replace("\n", ""); // Remove all line breaks
        }
        json = json.Replace("'", "\""); // Replace single quotes with double quotes
        if (writingToJsonFile)
        {
            json = json.StartsWith("\"{") ? json.Substring(1, json.Length - 2) : json; 
            json = json.Replace("\\\"", '\"'.ToString());
        }

        // Remove invalid characters (additional cleaning can be added here)
        json = System.Text.RegularExpressions.Regex.Replace(json, @"[^\u0000-\u007F]+", ""); // Remove non-ASCII characters

        return json;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}