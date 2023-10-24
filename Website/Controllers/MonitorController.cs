using System.Drawing;
using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarSpeedWebsite.Controllers;

[ApiController]
[Route("[controller]")]
public class MonitorController : ControllerBase
{
    private IHubContext<NotificationHub> _hubContext;
    public MonitorController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Returns latest preview image
    /// </summary>
    [HttpGet]
    [Route("PreviewImage")]
    public IActionResult PreviewImage() {
        MemoryStream ms = new MemoryStream(MonitorPreview.Instance.JpgImage);
        ms.Position = 0;
        FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
        return fr;
    }

    /// <summary>
    /// Returns latest preview image
    /// </summary>
    [HttpGet]
    [Route("PreviewState")]
    public MonitorState PreviewState() {
        return MonitorPreview.Instance.State;
    }

    /// <summary>
    /// Returns latest preview video
    /// </summary>
    [HttpGet]
    [Route("PreviewVideo")]
    public IActionResult PreviewVideo() {

        return new MjpegStreamContent(async cancellationToken => {
            return await MonitorPreview.Instance.GetNextImageAsync();
        });    
    }

    /// <summary>
    /// Start monitor
    /// </summary>
    [HttpGet]
    [Route("StartMonitor")]
    public IActionResult StartMonitor() {

        _hubContext.Clients.All.SendAsync("StartMonitor").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Stops monitor
    /// </summary>
    [HttpGet]
    [Route("StopMonitor")]
    public IActionResult StopMonitor() {

        _hubContext.Clients.All.SendAsync("StopMonitor").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Toggle detection
    /// </summary>
    [HttpGet]
    [Route("ToggleDetection")]
    public IActionResult ToggleDetection() {

        _hubContext.Clients.All.SendAsync("ToggleDetection").Wait();
        return this.Ok();        
    }

    /// <summary>
    /// Reset tracking
    /// </summary>
    [HttpGet]
    [Route("ResetTracking")]
    public IActionResult ResetTracking() {

        _hubContext.Clients.All.SendAsync("ResetTracking").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Get Monitor config
    /// </summary>
    [HttpGet]
    [Route("Config")]
    public MonitorConfig Config() {
        using ( var da = new DataAccess()) {
            var config = da.Monitor.GetMonitorConfig();
            return config;
        }
    }

    /// <summary>
    /// Post a new edit config
    /// </summary>
    [HttpPost]
    [Route("EditConfig")]
    public void EditConfig(MonitorConfig config) {
        using ( var da = new DataAccess()) {
            da.Monitor.Add(config);
            da.CommitChanges();
        }
        // send message so any other clients can load new config
         _hubContext.Clients.All.SendAsync("MonitorConfigEdited").Wait();
    }

}