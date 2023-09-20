using System.Security.Cryptography.X509Certificates;
using HaloSoft.DataAccess;
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

    public IList<Detection> GetAll() {
        return Session.Query<Detection>().ToList();
    }
    public Paged<Detection> GetFiltered(DetectionFilter filter) {
        var q = Session.Query<Detection>();
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
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.Direction) : q.OrderByDescending(m=>m.Direction);
            } else if ( filter.Sort == DetectionColumn.SD) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.SD) : q.OrderByDescending(m=>m.SD);
            } else if ( filter.Sort == DetectionColumn.Speed) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.Speed) : q.OrderByDescending(m=>m.Speed);
            } else if ( filter.Sort == DetectionColumn.Timestamp) {
                q = filter.SortDirection==SortDirection.Asc ? q.OrderBy(m=>m.DateTime) : q.OrderByDescending(m=>m.DateTime);
            }
        }
        //
        // find total
        var total = q.Count();
        // get list of items
        var data = q.Skip(filter.Skip).Take(filter.Take).ToList();
        // wrap in a paged object
        return new Paged<Detection>(data,total,filter.Skip,filter.Take);
    }

    public void Add(TrackingData td) {
        Session.Save(td);
    }

    public void Delete(TrackingData td) {
        Session.Delete(td);
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
    public int Skip {get; set;}
    public int Take {get; set;}

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