using System.Diagnostics;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherMVC.Models;
using WeatherMVC.Services;
using static IdentityModel.OidcConstants;

namespace WeatherMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenService _tokenService;

    public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
    {
        _logger = logger;
        this._tokenService = tokenService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [Authorize]
    public async Task<IActionResult> Weather()
    {
        using var client = new HttpClient();
        //use client-credential flow
        //var tokenResponse = await _tokenService.GetToken("weatherapi.read");
        //client.SetBearerToken(tokenResponse.AccessToken);

        //Use authorization code flow
        
        var token = await HttpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);
        var result = await client.GetAsync("https://localhost:5445/weatherforecast");
        if (result.IsSuccessStatusCode) {
            var model = await result.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<WeatherData>>(model);
            return View(data);
        }

        throw new Exception("Unable to get content");
    }
}
