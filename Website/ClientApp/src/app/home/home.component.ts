import { Component } from '@angular/core';
import { SignalRService } from '../signal-r/signal-r.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
    constructor(private signalRService: SignalRService) {

    }
}
