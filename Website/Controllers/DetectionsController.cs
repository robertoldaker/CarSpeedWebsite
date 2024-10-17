using CarSpeedWebsite.Data;
using CarSpeedWebsite.Models;
using HaloSoft.DataAccess;
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
            if ( resp!=null && resp.Length>0 ) {
                this.Response.ContentType = "image/jpg";
                MemoryStream ms = new MemoryStream(resp);
                ms.Position = 0;
                FileStreamResult fr = new FileStreamResult(ms, "image/jpg");
                return fr;
            } else {
                return this.Redirect("/NoImageAvailable.png");
            }
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

    /// <summary>
    /// Download detections as csv
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("DownloadAsCsv")]
    public ActionResult DownloadAsCsv([FromQuery] DetectionFilter filter)
    {
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetFilteredAsCsv(filter);
            return resp;
        }
    }

    [HttpGet]
    [Route("GroupedDetections")]
    public List<int> GetGroupedDetectionData( [FromQuery] DetectionGroups groups) {
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetGroupedDetectionData(groups);
            return resp;
        }
    }

    [HttpGet]
    [Route("NumberLastHour")]
    public int GetDetectionLastHour() {
        using( var da = new DataAccess()) {
            var resp = da.Detections.GetDetectionsLastHour();
            return resp;
        }
    }

    /// <summary>
    /// Backsup database to backup server
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("BackupDbLocally")]
    public IActionResult BackupDbLocally() {
        var m = new DatabaseBackup();
        var sr = m.BackupToStream(out string filename);
        var fsr = new FileStreamResult(sr.BaseStream, "application/sql");
        fsr.FileDownloadName = filename;
        return fsr;        
    }

    /// <summary>
    /// Purges tracking data from results if older than 7 days
    /// </summary>
    /// <returns></returns> <summary>
    [HttpGet]
    [Route("Purge")]
    public IActionResult Purge() {
        string mess;
        using ( var da = new DataAccess() ) {
            mess=da.Detections.Purge();
            da.CommitChanges();
        }
        return this.Ok(new {mess});
    }

    /// <summary>
    /// Returns data about the current database usage
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("DataModel")]
    public DataModel GetDataModel() {
        var m = new DataModel();
        m.Load();
        return m;
    }

    /// <summary>
    /// Performs a database cleanup operation to release diskspace (does a VACUUM FULL ANALYZE)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("PerformCleanup")]
    public void PerformCleanup() {
        DataAccessBase.PerformCleanup();
    }
}