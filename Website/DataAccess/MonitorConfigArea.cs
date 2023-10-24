using System.Text.Json.Serialization;
using HaloSoft.DataAccess;
using NHibernate.Mapping.Attributes;
namespace CarSpeedWebsite.Data;

[Class(0, Table = "monitor_config_areas")]
public class MonitorConfigArea {
    private string _class_name;

    public MonitorConfigArea() {
        _class_name = "MonitorArea";
    }

    /// <summary>
    /// Database identifier
    /// </summary>
    [Id(0, Name = "id", Type = "int")]
    [Generator(1, Class = "identity")]
    public virtual int id { get; set; }

    /// <summary>
    /// Name of class in Python environment
    /// </summary>
    /// <value></value>
    public virtual string class_name {
        get {
            return _class_name;
        }
    }

    /// <summary>
    /// Upper left x coord of monitor area
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual int upper_left_x {get; set;}

    /// <summary>
    /// Upper left y coord of monitor area
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual int upper_left_y {get; set;}

    /// <summary>
    /// Lower right x coord of monitor area
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual int lower_right_x {get; set;}

    /// <summary>
    /// Lower right y coord of monitor area
    /// </summary>
    /// <value></value>
    [Property()]
    public virtual int lower_right_y {get; set;}

}