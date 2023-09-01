import { Component } from '@angular/core';
import { DataService } from '../data/data-service.service';
import { Detection } from '../data/app.data';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-detections',
  templateUrl: './detections.component.html',
  styleUrls: ['./detections.component.css']
})
export class DetectionsComponent {
    constructor(private dataService: DataService) {
        this.detections=[]
        dataService.GetDetections( (resp)=>{
            this.detections = resp
            this.resultsLength = this.detections.length
        });
    }
    displayedColumns: string[] = ['timestamp', 'speed', 'direction', 'sd'];
    detections: Detection[]
    resultsLength = 0

    newPage(pageEvent: PageEvent) {
        console.log(pageEvent)

    }
}
