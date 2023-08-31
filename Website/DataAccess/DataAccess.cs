using CarSpeedWebsite.Controllers;
using HaloSoft.DataAccess;

namespace CarSpeedWebsite.Data;

public class DataAccess : DataAccessBase {
    public DataAccess() : base() {
        Detections = new Detections(this);
    }

    public Detections Detections {get; private set;}

    public static void SchemaUpdated(int oldVersion, int newVersion)
    {

    }

    public static void StartupScript(int oldVersion, int newVersion)
    {
        
    }
} 

