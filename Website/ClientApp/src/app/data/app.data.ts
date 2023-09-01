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
