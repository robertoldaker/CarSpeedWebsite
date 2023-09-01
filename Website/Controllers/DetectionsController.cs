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
    /// All
    /// </summary>
    [HttpGet]
    [Route("Filtered")]
    public Paged<Detection> GetFiltered([FromQuery] DetectionFilter filter) {    
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetFiltered(filter);
            return resp;
        }
    }
}
