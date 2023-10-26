using System.ComponentModel;
using System.Text.Json.Serialization;
using HaloSoft.DataAccess;
using NHibernate.Mapping.Attributes;
namespace CarSpeedWebsite.Data;

[Class(0, Table = "monitor_configs")]
public class MonitorConfig {
    private string _class_name;
    public MonitorConfig() {
        // Needed by Python environment
        _class_name = "CarSpeedConfig";
    }

    /// <summary>
    /// Database identifier
    /// </summary>
    [Id(0, Name = "id", Type = "int")]
    [Generator(1, Class = "identity")]
    public virtual int id { get; set; }

    /// <summary>
    /// Name of class on Python environment
    /// </summary>
    /// <value></value>    
    public virtual string class_name {
        get {
            return _class_name;
        }
    }

    /// <summary>
    /// Name of monitor the config is associated with
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual string name {get; set;}

    /// <summary>
    /// Left to right distance in m
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual float l2r_distance {get; set;}

    /// <summary>
    /// Right to left distance in m
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual float r2l_distance {get; set;}

    /// <summary>
    /// Minimum speed to record detection in mph
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual int min_speed_save {get; set;}

    /// <summary>
    /// Maximum speed to record detection in mph
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual int max_speed_save {get; set;} 

    /// <summary>
    /// Field of view of camera in deg.
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual float field_of_view {get; set;}

    /// <summary>
    /// Flip camera image horizontally
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual bool h_flip {get; set;}

    /// <summary>
    /// Flip camera image vertically
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual bool v_flip {get; set;}

    /// <summary>
    /// Area to monitor
    /// </summary>
    [ManyToOne(Column = "monitor_config_area_id", Cascade = "all-delete-orphan", Fetch = FetchMode.Join)]
    public virtual MonitorConfigArea monitor_area { get; set; }

}   