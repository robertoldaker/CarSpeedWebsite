import { EventEmitter, Injectable } from '@angular/core';
import { Detection, TrackingData } from '../data/app.data';
import { DataService } from '../data/data-service.service';

@Injectable({
    providedIn: 'root'
})
export class DetectionsService {

    constructor(private dataService: DataService) { }

    selected: Detection | null = null
    trackingData: TrackingData[] | null = null

    selectDetection(d: Detection|null) {
        if ( d ) {
            this.dataService.GetTrackingData(d.id, (resp)=>{
                this.selected = d;
                this.trackingData = resp
                this.selectionChanged.emit(d)
            })
        } else {
            this.selected = null;
            this.trackingData=null;    
        }
    }
        

    isSelected(d: Detection) {
        return this.selected?.id === d.id
    }

    selectionChanged: EventEmitter<Detection|null> = new EventEmitter()
}
