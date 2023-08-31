using System.Security.Cryptography.X509Certificates;
using HaloSoft.DataAccess;

namespace CarSpeedWebsite.Data;

public class Detections : DataSet {
    public Detections(DataAccessBase dab) : base(dab) {
        
    }   

    public void Add(Detection detection) {
        Session.Save(detection);
    }

    public void Delete(Detection detection) {
        // Remove tracking data linked to the detection
        var tds = Session.Query<TrackingData>().Where( m=>m.Detection==detection).ToList();
        foreach( var td in tds) {
            Session.Delete(td);
        }
        // Delete the actual detection
        Session.Delete(detection);

    }

    public void Add(TrackingData td) {
        Session.Save(td);
    }

    public void Delete(TrackingData td) {
        Session.Delete(td);
    }


}