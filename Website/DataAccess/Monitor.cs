using System.Security.Cryptography.X509Certificates;
using HaloSoft.DataAccess;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace CarSpeedWebsite.Data;

public class Monitor : DataSet {
    public Monitor(DataAccessBase dab) : base(dab) {
        
    }   

    public MonitorConfig GetMonitorConfig() {
        var config = Session.QueryOver<MonitorConfig>().OrderBy(m=>m.id).Desc.Take(1).SingleOrDefault();
        return config;
    }

    public void Add(MonitorConfig config) {
        Session.Save(config);
    }

    public static void Initialise() {
        // Ensur we have a default configuration
        using ( var da = new DataAccess() ) {
            var config = da.Monitor.GetMonitorConfig();
            if ( config==null ) {
                config = new MonitorConfig() 
                {
                    l2r_distance =  25,
                    r2l_distance =  17,
                    min_speed_save =  10,
                    max_speed_save =  80,
                    field_of_view =  75,
                    h_flip =  false,
                    v_flip =  false,
                    monitor_area =  new MonitorConfigArea() {
                        upper_left_x =  50,
                        upper_left_y =  50,
                        lower_right_x =  500,
                        lower_right_y =  300
                    }
                };
                da.Monitor.Add(config);
                da.CommitChanges();
            }
        }
    }
}