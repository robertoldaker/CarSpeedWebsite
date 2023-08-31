using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using CarSpeedWebsite.Data;

namespace CarSpeedWebsite.Models;

public class DetectionLoader {
    public DetectionLoader() {

    }

    public void Load(IFormFile file) {
        var regEx = new Regex(@"(\d).jpg$");
        using( var da = new DataAccess() ) {
            using( var stream = file.OpenReadStream()) {
                ZipArchive archive = new ZipArchive(stream);
                Detection d;
                List<TrackingData> td;
                // detection_Data.json
                var entry = archive.Entries.Where(m=>m.FullName.EndsWith("detection_data.json")).FirstOrDefault();
                if ( entry!=null ) {
                    using( var s=entry.Open()) {
                        d = loadDetectionData(da,s, out td);
                    }
                } else {
                    throw new Exception("Cannot find detection_data.json in zip file");
                }
                // detection_image.jpg
                entry = archive.Entries.Where(m=>m.FullName.EndsWith("detection_image.jpg")).FirstOrDefault();
                if ( entry!=null ) {
                    using( var s=entry.Open()) {
                        loadDetectionImage(da,s,entry.Length,d);
                    }
                } else {
                    throw new Exception("Cannot find detection_data.json in zip file");
                }
                // tracking images
                var entries = archive.Entries.Where(m=>regEx.IsMatch(m.FullName)).ToList();
                foreach( var e in entries) {
                    var m = regEx.Match(e.FullName);
                    var index = int.Parse(m.Groups[1].Value);
                    using( var s=e.Open()) {
                        loadTrackingImage(da,s,e.Length,td,index);
                    }
                }
            }
            //
            da.CommitChanges();
        }
    }

    private Detection loadDetectionData(DataAccess da, Stream stream, out List<TrackingData> trackingData) {
        var detectionResult = JsonSerializer.Deserialize<DetectionResult>(stream);
        if ( detectionResult!=null) {
            var d = detectionResult.createDetection(da, out trackingData);
            return d;
        } else {
            throw new Exception("Null result de-serializing detection_data.json");
        }
    }

    private void loadDetectionImage(DataAccess da, Stream stream, long len, Detection d) {
        using (MemoryStream ms = new MemoryStream((int) len))
        {
            stream.CopyTo(ms);
            d.Image = ms.ToArray();
        }        
    }

    private void loadTrackingImage(DataAccess da, Stream stream, long len, List<TrackingData> td, int index) {
        using (MemoryStream ms = new MemoryStream((int) len))
        {
            stream.CopyTo(ms);
            td[index].Image = ms.ToArray();
        }
    }
}

public class DetectionResult {
    public DetectionResult() {
        tracking_data = new List<RawTrackingData>();
    }
    public float posix_time {get; set;}
    public float mean_speed {get; set;}
    public DetectionDirection direction {get; set;}
    public float sd {get; set;}
    public bool inExitZone {get; set;}

    public Detection createDetection(DataAccess da, out List<TrackingData> trackingData) {
        var d = new Detection();
        d.DateTime = Utils.UnixTimeStampToDateTime(posix_time);
        d.Direction = direction;
        d.SD = sd;
        d.Speed = mean_speed;
        int index=0;
        trackingData = new List<TrackingData>();
        foreach( var rtd in tracking_data) {
            var td = new TrackingData(d);
            td.AbsChg = rtd.abs_chg;
            td.Speed = rtd.mph;
            td.Time = rtd.secs;
            td.X = rtd.x;
            td.Width = rtd.width;
            td.Index = index++;
            trackingData.Add(td);
            da.Detections.Add(td);
        }
        da.Detections.Add(d);
        return d;
    }

    public List<RawTrackingData> tracking_data {get; set;}
    public class RawTrackingData {
        public int abs_chg { get; set;}    
        public float secs {get; set;}
        public float mph {get; set;}
        public int x {get; set;}
        public int width {get; set;}
    }
}

