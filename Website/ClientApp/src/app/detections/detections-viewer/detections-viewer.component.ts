import { Component, Inject } from '@angular/core';
import { DetectionsService } from '../detections.service';
import { DetectionDirection, TrackingData } from 'src/app/data/app.data';

@Component({
  selector: 'app-detections-viewer',
  templateUrl: './detections-viewer.component.html',
  styleUrls: ['./detections-viewer.component.css']
})
export class DetectionsViewerComponent {
    constructor(public detectionsService: DetectionsService,@Inject('DATA_URL') private baseUrl: string) {
        this.detectionsService.selectionChanged.subscribe(resp=>{
            this.index=0;
            if ( this.detectionsService.trackingData ) {
                this.trackingData = this.detectionsService.trackingData[this.index];
            } else {
                this.trackingData=null;
            }
        })
    }

    get mainImageSrc():string {
        return this.detectionsService.selected ? `${this.baseUrl}/Detections/MainImage?id=${this.detectionsService.selected.id}` : ''
    }

    get trackingDataSrc():string {
        return this.trackingData ? `${this.baseUrl}/Detections/TrackingImage?id=${this.trackingData.id}` : ''
    }

    trackingData: TrackingData | null = null
    index: number = 0

    nextTrackingData() {
        if ( this.detectionsService.selected?.direction == DetectionDirection.LEFT_TO_RIGHT) {
            this.nextIndexTrackingData();
        } else {
            this.prevIndexTrackingData()
        }
    }

    prevTrackingData() {
        if ( this.detectionsService.selected?.direction == DetectionDirection.LEFT_TO_RIGHT) {
            this.prevIndexTrackingData();
        } else {
            this.nextIndexTrackingData()
        }
    }

    nextIndexTrackingData() {
        if ( this.detectionsService.trackingData!=null ) {
            if ( this.index<this.detectionsService.trackingData.length-1) {
                this.index++;
                //
                this.trackingData = this.detectionsService.trackingData[this.index];
            }
        }
    }
    prevIndexTrackingData() {
        if ( this.detectionsService.trackingData!=null ) {
            if ( this.index>=this.detectionsService.trackingData.length) {
                this.index=this.detectionsService.trackingData.length-1;
            }
            if ( this.index>0) {
                this.index--;
                //
                this.trackingData = this.detectionsService.trackingData[this.index];
            }
        }
    }

    get canDoPrev():boolean {
        if ( this.detectionsService.selected?.direction == DetectionDirection.LEFT_TO_RIGHT) {
            return this.index>0
        } else {
            if ( this.detectionsService && this.detectionsService.trackingData) {
                return this.index<this.detectionsService.trackingData.length-1
            } else {
                return false;
            }
        }
    }

    get canDoNext():boolean {
        if ( this.detectionsService.selected?.direction == DetectionDirection.LEFT_TO_RIGHT) {
            if ( this.detectionsService && this.detectionsService.trackingData) {
                return this.index<this.detectionsService.trackingData.length-1
            } else {
                return false;
            }
        } else {
            return this.index>0
        }        
    }

    getItemNumber():string {
        return this.detectionsService.trackingData ? `${this.index+1} of ${this.detectionsService.trackingData.length}` : '-'
    }

    DetectionDirection = DetectionDirection
}
