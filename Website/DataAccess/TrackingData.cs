using System.Text.Json.Serialization;
using HaloSoft.DataAccess;
using NHibernate.Mapping.Attributes;
namespace CarSpeedWebsite.Data;

[Class(0, Table = "tracking_data")]
public class TrackingData {
    public TrackingData() {

    }

    public TrackingData(Detection detection) {
        this.Detection = detection;
    }

    /// <summary>
    /// Database identifier
    /// </summary>
    [Id(0, Name = "Id", Type = "int")]
    [Generator(1, Class = "identity")]
    public virtual int Id { get; set; }

    [Property()]
    public virtual int AbsChg {get; set;}

    [Property()]
    public virtual int X {get; set;}

    [Property()]
    public virtual int Width {get; set;}

    [Property()]
    public virtual float Time {get; set;}

    [Property()]
    public virtual float Speed {get; set;}

    [Property()]
    public virtual int Index {get; set;}

    [JsonIgnore()]
    [Property(Type="BinaryBlob", Lazy=true)]
    public virtual byte[]? Image {get; set;}

    [JsonIgnore()]
    [ManyToOne(Column = "DetectionId", Cascade = "none")]
    public virtual Detection Detection {get; set;}
    
}