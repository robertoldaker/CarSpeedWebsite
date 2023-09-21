import { Component, Inject } from '@angular/core';
import { DetectionsService } from '../detections.service';
import { TrackingData } from 'src/app/data/app.data';

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
        if ( this.detectionsService.trackingData!=null ) {
            this.index++;
            if ( this.index>=this.detectionsService.trackingData.length) {
                this.index=0;
            }
            //
            this.trackingData = this.detectionsService.trackingData[this.index];
        }
    }
    prevTrackingData() {
        if ( this.detectionsService.trackingData!=null ) {
            if ( this.index>=this.detectionsService.trackingData.length) {
                this.index=this.detectionsService.trackingData.length-1;
            }
            this.index--;
            if ( this.index<0) {
                this.index=this.detectionsService.trackingData.length-1;
            }
            //
            this.trackingData = this.detectionsService.trackingData[this.index];
        }
    }
}
