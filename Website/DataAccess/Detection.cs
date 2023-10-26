using System.Text.Json.Serialization;
using HaloSoft.DataAccess;
using NHibernate.Mapping.Attributes;
namespace CarSpeedWebsite.Data;


public enum DetectionDirection { UNKNOWN, LEFT_TO_RIGHT, RIGHT_TO_LEFT }

[Class(0, Table = "detections")]
public class Detection {
    public Detection() {

    }

    /// <summary>
    /// Database identifier
    /// </summary>
    [Id(0, Name = "Id", Type = "int")]
    [Generator(1, Class = "identity")]
    public virtual int Id { get; set; }

    [Property()]
    public virtual DateTime DateTime {get; set;}

    [Property()]
    public virtual float Speed {get; set;}

    [Property()]
    public virtual DetectionDirection Direction {get; set;}

    [Property()]
    public virtual float SD {get; set;}

    [JsonIgnore()]
    [Property(Type="BinaryBlob", Lazy=true)]
    public virtual byte[]? Image {get; set;}

    [JsonIgnore()]
    [ManyToOne(Column = "MonitorConfigId", Cascade = "none")]
    public virtual MonitorConfig MonitorConfig {get; set;}

}   