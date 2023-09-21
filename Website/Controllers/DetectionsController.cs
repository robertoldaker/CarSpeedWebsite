using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarSpeedWebsite.Controllers;

[ApiController]
[Route("[controller]")]
public class DetectionsController : ControllerBase
{
    public DetectionsController()
    {

    }

    /// <summary>
    /// Uploads a detection zip file as produced from CarSpeed.py
    /// </summary>
    /// <param name="file">zip file containing detection</param>
    [HttpPost]
    [Route("Upload")]
    public void UploadDetectionZip(IFormFile file) {
        var m = new DetectionLoader();
        m.Load(file);
    }

    /// <summary>
    /// All
    /// </summary>
    [HttpGet]
    [Route("All")]
    public IList<Detection> GetAll() {    
        using( var da = new DataAccess()) {
            return da.Detections.GetAll();
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
            this.Response.ContentType = "image/jpg";
            MemoryStream ms = new MemoryStream(resp);
            ms.Position = 0;
            FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
            return fr;
        }
    }

}