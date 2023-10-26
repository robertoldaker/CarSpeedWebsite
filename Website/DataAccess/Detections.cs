using System.Security.Cryptography.X509Certificates;
using HaloSoft.DataAccess;
using NHibernate.Criterion;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

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

    public IList<Detection> GetAll(string monitorName) {
        var configIds = Session.QueryOver<MonitorConfig>().Where(m=>m.name==monitorName).Select(m=>m.id).List<int>().ToArray();
        return Session.QueryOver<Detection>().Where(m=>m.MonitorConfig.id.IsIn(configIds)).List();
    }

    public Paged<Detection> GetFiltered(DetectionFilter filter) {
        var configIds = Session.QueryOver<MonitorConfig>().Where(m=>m.name==filter.monitorName).Select(m=>m.id).List<int>().ToArray();

        var q = Session.QueryOver<Detection>().Where(m=>m.MonitorConfig.id.IsIn(configIds));
        // time stamp
        if ( filter.TimestampFilter!=null ) {
            // Dates come back as UTC so convert to local
            if ( filter.TimestampFilter.Type == FilterType.LessThan) {
                q = q.Where( m=>m.DateTime < filter.TimestampFilter.LessThan.ToLocalTime());
            } else if ( filter.TimestampFilter.Type == FilterType.MoreThan) {
                q = q.Where( m=>m.DateTime > filter.TimestampFilter.MoreThan.ToLocalTime());
            } else if ( filter.TimestampFilter.Type == FilterType.Between) {
                q = q.Where( m=>m.DateTime > filter.TimestampFilter.MoreThan.ToLocalTime()).
                      Where( m=>m.DateTime < filter.TimestampFilter.LessThan.ToLocalTime());
            }
        }
        // speed
        if ( filter.SpeedFilter!=null ) {
            if ( filter.SpeedFilter.Type == FilterType.LessThan) {
                q = q.Where( m=>m.Speed < filter.SpeedFilter.LessThan);
            } else if ( filter.SpeedFilter.Type == FilterType.MoreThan) {
                q = q.Where( m=>m.Speed > filter.SpeedFilter.MoreThan);
            } else if ( filter.SpeedFilter.Type == FilterType.Between) {
                q = q.Where( m=>m.Speed > filter.SpeedFilter.MoreThan).
                      Where( m=>m.Speed < filter.SpeedFilter.LessThan);
            }
        }
        // direction
        if ( filter.DirectionFilter!=null ) {
            if ( filter.DirectionFilter.Type == FilterType.Exactly) {
                q = q.Where( m=>m.Direction == filter.DirectionFilter.Exactly);
            } 
        }
        // sd
        if ( filter.SdFilter!=null ) {
            if ( filter.SdFilter.Type == FilterType.LessThan) {
                q = q.Where( m=>m.SD < filter.SdFilter.LessThan);
            } else if ( filter.SdFilter.Type == FilterType.MoreThan) {
                q = q.Where( m=>m.SD > filter.SdFilter.MoreThan);
            } else if ( filter.SdFilter.Type == FilterType.Between) {
                q = q.Where( m=>m.SD > filter.SdFilter.MoreThan).
                      Where( m=>m.SD < filter.SdFilter.LessThan);
            }
        }
        // sort columns
        if ( filter.SortDirection!=SortDirection.None) {
            if ( filter.Sort == DetectionColumn.Direction) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.Direction).Asc : q.OrderBy(m=>m.Direction).Desc;
            } else if ( filter.Sort == DetectionColumn.SD) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.SD).Asc : q.OrderBy(m=>m.SD).Desc;
            } else if ( filter.Sort == DetectionColumn.Speed) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.Speed).Asc : q.OrderBy(m=>m.Speed).Desc;
            } else if ( filter.Sort == DetectionColumn.Timestamp) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.DateTime).Asc : q.OrderBy(m=>m.DateTime).Desc;
            }
        }
        //
        // find total
        var total = q.RowCount();
        // get list of items
        var data = q.Skip(filter.Skip).Take(filter.Take).List();
        // wrap in a paged object
        return new Paged<Detection>(data,total,filter.Skip,filter.Take);
    }

    public byte[]? GetMainImage(int id) {
        var d = Session.Query<Detection>().Where(m=>m.Id==id).Select(m=>m.Image).FirstOrDefault();
        return d;
    }

    public void Add(TrackingData td) {
        Session.Save(td);
    }

    public void Delete(TrackingData td) {
        Session.Delete(td);
    }

    public IList<TrackingData> GetTrackingData(int detectionId) {
        var d = Session.Query<TrackingData>().Where(m=>m.Detection.Id==detectionId).OrderBy(m=>m.Index).ToList();
        return d;
    }

    public byte[]? GetTrackingImage(int id) {
        var d = Session.Query<TrackingData>().Where(m=>m.Id==id).Select(m=>m.Image).FirstOrDefault();
        return d;
    }

}

public enum DetectionColumn {
    Timestamp,
    Speed,
    Direction,
    SD
}

public enum FilterType {
    Exactly,
    LessThan,
    MoreThan,
    Between
}

public enum SortDirection {
    None,
    Asc,
    Desc
}

public class DetectionFilter {
    public DetectionFilter() {
        monitorName="";
    }
    public int Skip {get; set;}
    public int Take {get; set;}
    public string monitorName {get; set;}
    public ColumnFilter<DateTime>? TimestampFilter {get; set;}
    public ColumnFilter<float>? SpeedFilter {get; set;}
    public ColumnFilter<DetectionDirection>? DirectionFilter {get; set;}
    public ColumnFilter<float>? SdFilter {get; set;}

    public DetectionColumn Sort {get; set;}
    public SortDirection SortDirection {get; set;}
}

public class ColumnFilter<T> {
    public ColumnFilter() {
    }
    public FilterType? Type {get; set;}
    public T? Exactly {get; set;}
    public T? LessThan {get; set;}
    public T? MoreThan {get; set;}
}

public class Paged<T> {
    public Paged(IList<T> data, int total, int skip, int take) {
        Data = data;
        Skip = skip;
        Take = take;
        Total = total;
    }
    public IList<T> Data {get; set;}
    public int Skip {get; set;}
    public int Take {get; set;}
    public int Total {get; set;}
}