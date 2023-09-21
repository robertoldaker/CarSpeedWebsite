import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FilterTableNumericDialogComponent } from '../filter-table-numeric-dialog/filter-table-numeric-dialog.component';
import { FilterTableDateDialogComponent } from '../filter-table-date-dialog/filter-table-date-dialog.component';


export enum SortState {
    None,
    Asc,
    Desc
}

@Component({
  selector: 'app-filter-table-header',
  templateUrl: './filter-table-header.component.html',
  styleUrls: ['./filter-table-header.component.css']
})
export class FilterTableHeaderComponent {

    constructor(public dialog: MatDialog) {
        this.colData=new ColData(0,"")
    }

    @Input()
    colData: ColData

    @Output()
    sortChanged: EventEmitter<SortChangeEvent> = new EventEmitter()
    
    @Output()
    filterChanged: EventEmitter<FilterChangeEvent> = new EventEmitter()

    toggleSort() {
        if ( this.colData) {
            if ( this.colData.sort==SortState.None) {
                this.colData.sort=SortState.Asc
            } else if (this.colData.sort==SortState.Asc) {
                this.colData.sort=SortState.Desc
            } else if (this.colData.sort==SortState.Desc) {
                this.colData.sort=SortState.None
            }
            if (this.sortChanged) {
                this.sortChanged.emit(new SortChangeEvent(this.colData.sort,this.colData.id));
            }    
        }
    }

    openFilterDialog() {
        if ( this.colData && this.colData.filterType == this.FilterType.Numeric) {
            const dialogRef = this.dialog.open(FilterTableNumericDialogComponent,{ data: this.colData}); 
            dialogRef.afterClosed().subscribe(colData => {
                if ( colData ) {
                    this.filterChanged.emit(new FilterChangeEvent(colData.columnFilter,colData.id))
                }
            });                 
        } else if ( this.colData && this.colData.filterType == this.FilterType.Date) {
            const dialogRef = this.dialog.open(FilterTableDateDialogComponent,{ data: this.colData}); 
            dialogRef.afterClosed().subscribe(colData => {
                if ( colData ) {
                    this.filterChanged.emit(new FilterChangeEvent(colData.columnFilter,colData.id))
                }
            });                 
        }
    }

    menuItemClicked(e: ColDataMenuOption) {
        if ( this.filterChanged && this.colData) {
            this.colData.columnFilter = ColDataFilter.NewExactly(e.id)
            this.colData.isActive = true;
            this.filterChanged.emit( new FilterChangeEvent(this.colData.columnFilter,this.colData.id))
        }
    }

    clear() {
        if ( this.filterChanged && this.colData) {
            this.colData.isActive=false;
            this.filterChanged.emit( new FilterChangeEvent(null,this.colData.id))
        }
    }
    SortState = SortState
    FilterType = ColDataFilterType
}

export enum ColDataFilterType {
    None,
    Menu,
    Numeric,
    Date
}

export class ColData {
    constructor(public id: number, public title: string, filterType: ColDataFilterType=ColDataFilterType.None, menuOptions: ColDataMenuOption[]|undefined=undefined) {        
        this.name = title.toLowerCase()
        this.sort = SortState.None
        this.filterType = filterType
        this.menuOptions = menuOptions
        this.columnFilter = new ColDataFilter()
        this.isActive = false
        this.numActiveColumns = 0
    }
    name: string
    sort: SortState
    get hasFilter():boolean {
        return this.filterType!=ColDataFilterType.None
    }
    filterType: ColDataFilterType
    menuOptions: ColDataMenuOption[] | undefined
    columnFilter: ColDataFilter
    isActive: boolean
    numActiveColumns: number
    getFilterText():string {
        function toDateStr(date: Date) {
            let fmt = new Intl.DateTimeFormat("en-UK").format(date);
            return fmt;
        }
        let f = this.columnFilter;
        let str=''
        if ( this.filterType == ColDataFilterType.Date ) {
            if ( f.type == ColDataValueType.LessThan) {
                str=`Before ${toDateStr(f.lessThan)}`
            } else if ( f.type == ColDataValueType.Between) {
                str=`${toDateStr(f.moreThan)} and ${toDateStr(f.lessThan)}`
            } else if ( f.type == ColDataValueType.MoreThan) {
                str=`After ${toDateStr(f.moreThan)}`
            }  
        } else {
            if ( f.type == ColDataValueType.LessThan) {
                str=`< ${f.lessThan}`
            } else if ( f.type == ColDataValueType.Between) {
                str=`${f.moreThan} and ${f.lessThan}`
            } else if ( f.type == ColDataValueType.MoreThan) {
                str=`> ${f.moreThan}`
            } else if ( f.type == ColDataValueType.Exactly && this.menuOptions) {
                str=this.menuOptions[f.exactly].name
            }    
        }
        return str;
    }

    isValid():boolean {
        return this.columnFilter.isValid()
    }

}

export enum ColDataValueType {
    Exactly,
    LessThan,
    MoreThan,
    Between
}

export class ColDataFilter {
    constructor() {
        this.type = ColDataValueType.LessThan
    }
    public static NewExactly(exactly: any):ColDataFilter {
        let f = new ColDataFilter()
        f.type=ColDataValueType.Exactly
        f.exactly = exactly
        return f
    }
    isValidValue(v: any):boolean {
        return (v || v===0)
    }
    isValid():boolean {
        if ( this.type ==ColDataValueType.LessThan && this.isValidValue(this.lessThan)) {
            return true;
        }
        else if ( this.type ==ColDataValueType.Between && this.isValidValue(this.lessThan) && this.isValidValue(this.moreThan)) {
            return true;
        }
        else if ( this.type ==ColDataValueType.MoreThan && this.isValidValue(this.moreThan)) {
            return true;
        }
        else if ( this.type ==ColDataValueType.Exactly && this.isValidValue(this.exactly)) {
            return true;
        } else {
            return false;
        }
    }
    type: ColDataValueType
    exactly:any
    lessThan:any
    moreThan:any
}

export interface ColDataMenuOption {
    name: string
    id: number
}

export class SortChangeEvent {
    constructor(public sort: SortState, public colId: number) {}
}

export class FilterChangeEvent {
    constructor(public filter: ColDataFilter | null, public colId: number) {}
}
