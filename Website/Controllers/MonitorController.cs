using System.Drawing;
using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NHibernate.Criterion;
using NHibernate.Linq;
using Org.BouncyCastle.Crypto.Signers;

namespace CarSpeedWebsite.Controllers;

[ApiController]
[Route("[controller]")]
public class MonitorController : ControllerBase
{
    private IHubContext<NotificationHub> _hubContext;
    private static object _configLock = new object();
    public MonitorController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Returns latest preview image
    /// </summary>
    [HttpGet]
    [Route("PreviewImage")]
    public IActionResult PreviewImage(string monitorName) {
        MemoryStream ms = new MemoryStream(MonitorPreview.Instance.GetJpgImage(monitorName));
        ms.Position = 0;
        FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
        return fr;
    }

    /// <summary>
    /// Returns latest preview image
    /// </summary>
    [HttpGet]
    [Route("PreviewState")]
    public MonitorState? PreviewState(string monitorName) {
        return MonitorPreview.Instance.GetState(monitorName);
    }

    /// <summary>
    /// Returns latest preview video
    /// </summary>
    [HttpGet]
    [Route("PreviewVideo")]
    public IActionResult PreviewVideo(string monitorName) {

        return new MjpegStreamContent(async cancellationToken => {
            return await MonitorPreview.Instance.GetNextImageAsync(monitorName);
        });    
    }

    /// <summary>
    /// Start monitor
    /// </summary>
    [HttpGet]
    [Route("StartMonitor")]
    public IActionResult StartMonitor(string monitorName) {

        var connectionIds = ConnectionManager.Instance.GetMonitorIds(monitorName);
        _hubContext.Clients.Clients(connectionIds).SendAsync("StartMonitor").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Stops monitor
    /// </summary>
    [HttpGet]
    [Route("StopMonitor")]
    public IActionResult StopMonitor(string monitorName) {

        var connectionIds = ConnectionManager.Instance.GetMonitorIds(monitorName);
        _hubContext.Clients.Clients(connectionIds).SendAsync("StopMonitor").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Toggle detection
    /// </summary>
    [HttpGet]
    [Route("ToggleDetection")]
    public IActionResult ToggleDetection(string monitorName) {

        var connectionIds = ConnectionManager.Instance.GetMonitorIds(monitorName);
        _hubContext.Clients.Clients(connectionIds).SendAsync("ToggleDetection").Wait();
        return this.Ok();        
    }

    /// <summary>
    /// Reset tracking
    /// </summary>
    [HttpGet]
    [Route("ResetTracking")]
    public IActionResult ResetTracking(string monitorName) {
        var connectionIds = ConnectionManager.Instance.GetMonitorIds(monitorName);
        _hubContext.Clients.Clients(connectionIds).SendAsync("ResetTracking").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Shutdown
    /// </summary>
    [HttpGet]
    [Route("Shutdown")]
    public IActionResult Shutdown(string monitorName) {
        var connectionIds = ConnectionManager.Instance.GetMonitorIds(monitorName);
        _hubContext.Clients.Clients(connectionIds).SendAsync("Shutdown").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Reset tracking
    /// </summary>
    [HttpGet]
    [Route("Reboot")]
    public IActionResult Reboot(string monitorName) {
        var connectionIds = ConnectionManager.Instance.GetMonitorIds(monitorName);
        _hubContext.Clients.Clients(connectionIds).SendAsync("Reboot").Wait();
        return this.Ok();
    }

    /// <summary>
    /// Get Monitor config
    /// </summary>
    [HttpGet]
    [Route("Config")]
    public MonitorConfig Config(string monitorName) {
        lock( _configLock) {
            if ( monitorName == null) {
                throw new Exception("Attempt to get a config with null monitor name");
            }
            using ( var da = new DataAccess()) {
                var config = da.Monitor.GetMonitorConfig(monitorName,out bool needsCommit);
                if ( needsCommit ) {
                    da.CommitChanges();
                }
                return config;
            }
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
        // and monitors can restart
        var allMonitors = ConnectionManager.Instance.GetAllMonitorIds();
         _hubContext.Clients.AllExcept(allMonitors).SendAsync("MonitorConfigEdited",new {monitorName = config.name}).Wait();
         var monitorIds = ConnectionManager.Instance.GetMonitorIds(config.name);
         _hubContext.Clients.Clients(monitorIds).SendAsync("MonitorConfigEdited",new {monitorName = config.name}).Wait();
    }

    /// <summary>
    /// Get list of monitors
    /// </summary>
    [HttpGet]
    [Route("MonitorInfo")]
    public List<Data.Monitor.MonitorInfo> MonitorInfo() {
        using ( var da = new DataAccess()) {
            var list = da.Monitor.GetMonitorInfo();
            return list;
        }
    }

}