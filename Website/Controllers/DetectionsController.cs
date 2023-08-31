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

}
