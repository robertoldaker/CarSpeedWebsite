/**
 * Data definitions
 */
export enum DetectionDirection {UNKNOWN,LEFT_TO_RIGHT,RIGHT_TO_LEFT}

export interface Detection {
    id: number,
    dateTime: Date,
    speed: number,
    direction:DetectionDirection,
    sd:number
}

export interface TrackingData {
    id:number,
    absChg:number,
    x:number,
    width:number,
    time: number,
    speed: number,
    index: number,
}


export interface Paged<T> {
    total: number
    data: T[]
    skip: number,
    take: number,
}

export enum DetectionColumn {
    Timestamp,
    Speed,
    Direction,
    SD
}

export enum FilterType {
    Exactly,
    LessThan,
    MoreThan,
    Between
}

export enum SortDirection {
    None,
    Asc,
    Desc
}

export interface DetectionFilter {
    skip: number,
    take: number,

    timestampFilter: ColumnFilter<string> | undefined
    speedFilter: ColumnFilter<number> | undefined
    directionFilter: ColumnFilter<DetectionDirection> | undefined
    sdFilter: ColumnFilter<number> | undefined

    sort: DetectionColumn
    sortDirection: SortDirection
}

export interface ColumnFilter<T> {
    type: FilterType
    exactly:T | undefined
    lessThan:T | undefined
    moreThan:T | undefined
}

export interface MonitorState {
    state: string
    frameRate: number
    detectionEnabled: boolean
    avgContours: number
}