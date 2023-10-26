using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CarSpeedWebsite.Controllers;

[ApiController]
[Route("[controller]")]
public class DetectionsController : ControllerBase
{
    private IHubContext<NotificationHub> _hubContext;
    public DetectionsController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Uploads a detection zip file as produced from CarSpeed.py
    /// </summary>
    /// <param name="file">zip file containing detection</param>
    [HttpPost]
    [Route("Upload")]
    public void UploadDetectionZip(IFormFile file) {
        var m = new DetectionLoader();
        m.Load(file, out string? monitorName);
        // Send out signal r message
        if ( monitorName!=null ) {
            var monitors = ConnectionManager.Instance.GetAllMonitorIds();
            _hubContext.Clients.AllExcept(monitors).SendAsync("NewDetectionLoaded",new {monitorName=monitorName});
        }
    }

    /// <summary>
    /// All
    /// </summary>
    [HttpGet]
    [Route("All")]
    public IList<Detection> GetAll(string monitorName) {    
        using( var da = new DataAccess()) {
            return da.Detections.GetAll(monitorName);
        }
    }

    /// <summary>
    /// Filtered detections
    /// </summary>
    [HttpGet]
    [Route("Filtered")]
    public Paged<Detection> GetFiltered([FromQuery] DetectionFilter filter) {    
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetFiltered(filter);
            return resp;
        }
    }

    /// <summary>
    /// Main image
    /// </summary>
    [HttpGet]
    [Route("MainImage")]
    public IActionResult GetMainImage([FromQuery] int id) {    
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetMainImage(id);
            this.Response.ContentType = "image/jpg";
            MemoryStream ms = new MemoryStream(resp);
            ms.Position = 0;
            FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
            return fr;
        }
    }

    /// <summary>
    /// Tracking data
    /// </summary>
    /// <param name="detectionId">Id of detection tracking data is associated with</param>
    /// <returns>Array of TrackingData objects</returns>
    [HttpGet]
    [Route("TrackingData")]
    public IList<TrackingData> GetTrackingData([FromQuery] int detectionId) {    
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetTrackingData(detectionId);
            return resp;
        }
    }

    /// <summary>
    /// Main image
    /// </summary>
    [HttpGet]
    [Route("TrackingImage")]
    public IActionResult GetTrackingImage([FromQuery] int id) {    
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetTrackingImage(id);
            if ( resp!=null) {
                this.Response.ContentType = "image/jpg";
                MemoryStream ms = new MemoryStream(resp);
                ms.Position = 0;
                FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
                return fr;
            } else {
                return this.Ok();
            }
        }
    }

}