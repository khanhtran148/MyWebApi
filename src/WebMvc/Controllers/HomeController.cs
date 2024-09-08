using System.Diagnostics;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Application.Languages.Queries;
using MyWebApi.Domain.Common;
using WebMvc.Models;

namespace WebMvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMediator _mediator;

    public HomeController(ILogger<HomeController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        Result<List<Select2ItemResponse>> result = await _mediator.Send(new GetLanguages.Request() { IncludeDisabled = true }, cancellationToken);
        _logger.LogInformation("hello from home controller");
        return View(result.Value);
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
}
