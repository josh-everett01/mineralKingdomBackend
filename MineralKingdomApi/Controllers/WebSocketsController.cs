using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using MineralKingdomApi.Services;
using MineralKingdomApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebSocketsController : ControllerBase
{
    private readonly AppWebSocketsManager _appWebSocketManager;
    private readonly ILogger<WebSocketsController> _logger;

    public WebSocketsController(AppWebSocketsManager appWebSocketManager, ILogger<WebSocketsController> logger)
    {
        _appWebSocketManager = appWebSocketManager;
        _logger = logger;
    }

    [HttpGet("/ws")]
    public async Task Get()
    {
        _logger.LogInformation(HttpContext.TraceIdentifier);
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var id = Guid.NewGuid(); // Generate a unique ID for the socket
            await _appWebSocketManager.Handle(id, webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}
