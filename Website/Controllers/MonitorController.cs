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
}