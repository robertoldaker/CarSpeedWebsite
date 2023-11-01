import { Component, Inject, QueryList, ViewChildren } from '@angular/core';
import { PageEvent } from '@angular/material/paginator';
import { HttpParams } from '@angular/common/http';
import { DetectionColumn, DetectionDirection, Detection, SortDirection, FilterType, DetectionFilter, ColumnFilter } from 'src/app/data/app.data';
import { DataService } from 'src/app/data/data-service.service';
import { ColData, ColDataFilterType, FilterTableHeaderComponent, SortChangeEvent, SortState, FilterChangeEvent, ColDataFilter, ColDataValueType } from 'src/app/shared/filter-table-header/filter-table-header.component';
import { DetectionsService } from '../detections.service';
import { SignalRService } from 'src/app/signal-r/signal-r.service';
import { MonitorService } from 'src/app/monitor/monitor.service';

@Component({
    selector: 'app-detections-table',
    templateUrl: './detections-table.component.html',
    styleUrls: ['./detections-table.component.css']
})
export class DetectionsTableComponent {
    constructor(private dataService: DataService, 
        private detectionsService: DetectionsService, 
        private signalRService: SignalRService, 
        private monitorService: MonitorService,
        @Inject('DATA_URL') private baseUrl: string) {
        this.colDataMap.set(DetectionColumn.Timestamp, new ColData(DetectionColumn.Timestamp, "Timestamp", ColDataFilterType.Date))
        this.colDataMap.set(DetectionColumn.Speed, new ColData(DetectionColumn.Speed, "Speed", ColDataFilterType.Numeric))
        this.colDataMap.set(DetectionColumn.Direction, new ColData(DetectionColumn.Direction, "Direction", ColDataFilterType.Menu,
            [{ name: "Unknown", id: DetectionDirection.UNKNOWN },
            { name: "Left to right", id: DetectionDirection.LEFT_TO_RIGHT },
            { name: "Right to left", id: DetectionDirection.RIGHT_TO_LEFT }
            ]))
        this.colDataMap.set(DetectionColumn.SD, new ColData(DetectionColumn.SD, "SD", ColDataFilterType.Numeric))
        this.displayedColumns = Array.from(this.colDataMap.values()).map(m => m.name)
        this.detections = []
        this.filter.monitorName = this.monitorService.selectedMonitorName
        if (this.filter.monitorName) {
            this.loadData()
        }
        this.signalRService.hubConnection.on("NewDetectionLoaded", (data) => {
            // reload data if its for the selected monitor
            if ( data.monitorName===this.monitorService.selectedMonitorName) {
                this.loadData()
            }
        })
        this.monitorService.SelectecMonitorChange.subscribe((data)=>{
            this.filter.monitorName = data.name
            this.loadData()
        })       
    }
    @ViewChildren('filterHeader', { read: FilterTableHeaderComponent }) filterHeaders: QueryList<FilterTableHeaderComponent> | undefined

    colDataMap = new Map<DetectionColumn, ColData>();
    displayedColumns: string[];
    detections: Detection[]
    total = 0
    pageSize = 30
    pageIndex = 0;
    filter: DetectionFilterImp = new DetectionFilterImp(this.pageSize)

    newPage(pageEvent: PageEvent) {
        this.filter.skip = pageEvent.pageIndex * this.pageSize;
        this.pageIndex = pageEvent.pageIndex
        this.loadData()
    }

    loadData() {
        this.dataService.GetFilteredDetections(this.filter, (resp) => {
            this.detections = resp.data
            this.total = resp.total
        });
    }

    sortChanged(e: SortChangeEvent) {
        if (e.sort == SortState.Asc) {
            this.filter.sortDirection = SortDirection.Asc
        } else if (e.sort == SortState.Desc) {
            this.filter.sortDirection = SortDirection.Desc
        }
        this.filter.sort = e.colId
        this.filter.skip = 0
        this.pageIndex = 0
        // turn off sorting of other columns
        if (this.filterHeaders) {
            this.filterHeaders.forEach(fh => {
                if (fh.colData) {
                    if (fh.colData.id != e.colId) {
                        fh.colData.sort = SortState.None
                    }
                }
            })
        }
        this.loadData()
    }

    getColData(id: number): ColData {
        let colData = this.colDataMap.get(id);
        if (!colData) {
            colData = new ColData(0, "")
        }
        return colData;
    }

    filterChanged(e: FilterChangeEvent) {
        let colData = this.colDataMap.get(e.colId);
        this.filter.skip = 0
        this.pageIndex = 0
        if (e.filter == null) {
            if (e.colId == DetectionColumn.Speed) {
                this.filter.speedFilter = undefined;
            } else if (e.colId == DetectionColumn.SD) {
                this.filter.sdFilter = undefined;
            } else if (e.colId == DetectionColumn.Timestamp) {
                this.filter.timestampFilter = undefined;
            } else if (e.colId == DetectionColumn.Direction) {
                this.filter.directionFilter = undefined;
            }
            this.loadData();
        }
        else if (colData && colData.filterType == ColDataFilterType.Numeric) {
            let filter = this.getNumberFilter(e.filter);
            if (e.colId == DetectionColumn.Speed) {
                this.filter.speedFilter = filter;
            } else if (e.colId == DetectionColumn.SD) {
                this.filter.sdFilter = filter;
            }
            this.loadData();
        } else if (colData && colData.filterType == ColDataFilterType.Date) {
            let filter = this.getDateFilter(e.filter);
            if (e.colId == DetectionColumn.Timestamp) {
                this.filter.timestampFilter = filter;
            }
            this.loadData();
        } else if (colData && colData.filterType == ColDataFilterType.Menu) {
            if (e.colId == DetectionColumn.Direction) {
                let filter = new ColumnFilterImp<DetectionDirection>(FilterType.Exactly, e.filter.exactly);
                this.filter.directionFilter = filter;
            }
            this.loadData();
        }
        // Update number of active columns
        if (this.filterHeaders) {
            let numActiveColumns = this.filterHeaders.filter(m => m.colData.isActive).length;
            this.filterHeaders.forEach(fh => {
                fh.colData.numActiveColumns = numActiveColumns
            })
        }
    }

    getNumberFilter(f: ColDataFilter): ColumnFilterImp<number> {
        let filter
        if (f.type == ColDataValueType.LessThan) {
            filter = new ColumnFilterImp<number>(FilterType.LessThan, f.lessThan)
        } else if (f.type == ColDataValueType.Between) {
            filter = new ColumnFilterImp<number>(FilterType.Between, f.lessThan, f.moreThan)
        } else {
            filter = new ColumnFilterImp<number>(FilterType.MoreThan, f.moreThan)
        }
        return filter;
    }

    getDateFilter(f: ColDataFilter): ColumnFilterImp<string> {
        function toJson(v: Date): string {
            let json = v.toJSON();
            return json;
        }
        let filter
        if (f.type == ColDataValueType.LessThan) {
            filter = new ColumnFilterImp<string>(FilterType.LessThan, toJson(f.lessThan))
        } else if (f.type == ColDataValueType.Between) {
            filter = new ColumnFilterImp<string>(FilterType.Between, toJson(f.lessThan), toJson(f.moreThan))
        } else {
            filter = new ColumnFilterImp<string>(FilterType.MoreThan, toJson(f.moreThan))
        }
        return filter;
    }

    rowSelected(d: Detection) {
        this.detectionsService.selectDetection(d)
    }

    isSelected(d: Detection) {
        return this.detectionsService.isSelected(d)
    }

    download() {
        let params=this.filter.getHttpParams()
        let url = this.baseUrl + "/Detections/DownloadAsCsv?" + params.toString();
        document.location.href = url
    }

    DetectionColumn = DetectionColumn
    DetectionDirection = DetectionDirection
}

export class DetectionFilterImp implements DetectionFilter {
    constructor(pageSize: number) {
        this.skip = 0
        this.take = pageSize
        this.sort = DetectionColumn.Timestamp
        this.sortDirection = SortDirection.Desc
        this.monitorName = ''
    }
    skip: number
    take: number
    monitorName: string
    timestampFilter: ColumnFilterImp<string> | undefined
    speedFilter: ColumnFilterImp<number> | undefined
    directionFilter: ColumnFilterImp<DetectionDirection> | undefined
    sdFilter: ColumnFilterImp<number> | undefined
    sort: DetectionColumn
    sortDirection: SortDirection

    getHttpParams(): HttpParams {
        let p = new HttpParams();
        p = p.append("skip", this.skip)
        p = p.append("take", this.take)
        p = p.append("monitorName",this.monitorName)
        if (this.timestampFilter) {
            let root = "timestampFilter."
            p = this.timestampFilter.addHttpParams(p, root)
        }
        if (this.speedFilter) {
            let root = "speedFilter."
            p = this.speedFilter.addHttpParams(p, root)
        }
        if (this.directionFilter) {
            let root = "directionFilter."
            p = this.directionFilter.addHttpParams(p, root)
        }
        if (this.sdFilter) {
            let root = "sdFilter."
            p = this.sdFilter.addHttpParams(p, root)
        }
        p = p.append("sort", this.sort)
        p = p.append("sortDirection", this.sortDirection)
        return p
    }
}

export class ColumnFilterImp<T> implements ColumnFilter<T> {
    constructor(t: FilterType, v1: T, v2: T | undefined = undefined) {
        this.type = t
        if (this.type == FilterType.Between && v2 != undefined) {
            this.lessThan = v1;
            this.moreThan = v2;
        } else if (this.type == FilterType.Exactly) {
            this.exactly = v1;
        } else if (this.type == FilterType.LessThan) {
            this.lessThan = v1;
        } else if (this.type == FilterType.MoreThan) {
            this.moreThan = v1;
        }
    }
    type: FilterType;
    exactly: T | undefined;
    lessThan: T | undefined;
    moreThan: T | undefined;

    addHttpParams(p: HttpParams, root: string): HttpParams {
        p = p.append(root + "type", this.type)
        if (this.exactly) {
            p = p.append(root + "exactly", this.exactly.toString())
        }
        if (this.lessThan) {
            p = p.append(root + "lessThan", this.lessThan.toString())
        }
        if (this.moreThan) {
            p = p.append(root + "moreThan", this.moreThan.toString())
        }
        return p;
    }


}
