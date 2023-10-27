using System.Security.Cryptography.X509Certificates;
using CarSpeedWebsite.Models;
using HaloSoft.DataAccess;
using NHibernate.Criterion;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace CarSpeedWebsite.Data;

public class Monitor : DataSet {
    public Monitor(DataAccessBase dab) : base(dab) {
        
    }   

    public MonitorConfig GetMonitorConfig(int id) {
        var config = Session.QueryOver<MonitorConfig>().Where(m=>m.id==id).Take(1).SingleOrDefault();
        return config;
    }

    public MonitorConfig GetMonitorConfig(string monitorName, out bool needsCommit) {
        needsCommit = false;
        var config = Session.QueryOver<MonitorConfig>().Where(m=>m.name.IsLike(monitorName)).OrderBy(m=>m.id).Desc.Take(1).SingleOrDefault();
        if ( config==null) {
            needsCommit = true;
            config = new MonitorConfig() 
            {
                name=monitorName,
                l2r_distance =  7.62f, // in m
                r2l_distance =  5.18f, // in m
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
            Session.Save(config);
        }
        return config;
    }

    public void Add(MonitorConfig config) {
        Session.Save(config);
    }

    public class MonitorInfo {
        public MonitorInfo(string name, bool isConnected) {
            Name = name;
            IsConnected= isConnected;
        }
        public string Name {get; set;}
        public bool IsConnected {get; set;}
    }

    public List<MonitorInfo> GetMonitorInfo() {
        var names=Session.QueryOver<MonitorConfig>().SelectList(l=>l.SelectGroup(m=>m.name)).List<string>();
        var list = new List<MonitorInfo>();
        foreach( var name in names) {
            if ( name!=null ) {
                list.Add(new MonitorInfo(name,ConnectionManager.Instance.IsConnected(name)));
            }
        }
        return list;
    }
}