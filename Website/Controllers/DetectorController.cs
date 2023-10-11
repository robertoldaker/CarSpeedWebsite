using System.Drawing;
using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarSpeedWebsite.Controllers;

[ApiController]
[Route("[controller]")]
public class DetectorController : ControllerBase
{
    private IHubContext<NotificationHub> _hubContext;
    public DetectorController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Returns latest preview image
    /// </summary>
    [HttpGet]
    [Route("PreviewImage")]
    public IActionResult PreviewImage() {
        MemoryStream ms = new MemoryStream(DetectorPreview.Instance.State.jpg);
        ms.Position = 0;
        FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
        return fr;
    }

    /// <summary>
    /// Returns latest preview image
    /// </summary>
    [HttpGet]
    [Route("PreviewState")]
    public DetectorState PreviewState() {
        return DetectorPreview.Instance.State;
    }

    /// <summary>
    /// Returns latest preview video
    /// </summary>
    [HttpGet]
    [Route("PreviewVideo")]
    public IActionResult PreviewVideo() {

        return new MjpegStreamContent(async cancellationToken => {
            return await DetectorPreview.Instance.GetNextImageAsync();
        });    
    }
}