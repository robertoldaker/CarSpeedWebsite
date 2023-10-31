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
    monitorName: string,
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
    lightLevel: number,
    exposureTime: number,
    analogueGain: number,
    cpus: number[]
}

export interface MonitorConfig {
    l2r_distance: number,
    r2l_distance: number,
    min_speed_save: number,
    max_speed_save: number,
    field_of_view: number,
    h_flip: boolean,
    v_flip: boolean,
    monitor_area: MonitorConfigArea
}

export interface MonitorConfigArea {
    upper_left_x: number,
    upper_left_y:number,
    lower_right_x:number,
    lower_right_y:number
}

export interface MonitorInfo {
    name:string,
    isConnected:boolean
}