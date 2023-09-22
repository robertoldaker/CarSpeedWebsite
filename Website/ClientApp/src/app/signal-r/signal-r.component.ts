import { Component } from '@angular/core';
import { SignalRService } from './signal-r.service';

@Component({
  selector: 'app-signal-r',
  templateUrl: './signal-r.component.html',
  styleUrls: ['./signal-r.component.css']
})
export class SignalRComponent {
    constructor(public service:SignalRService) {
        
    }
}
