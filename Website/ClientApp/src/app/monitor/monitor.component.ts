import { Component, Inject } from '@angular/core';
import { SignalRService } from '../signal-r/signal-r.service';
import { MonitorState } from '../data/app.data';

@Component({
  selector: 'app-monitor',
  templateUrl: './monitor.component.html',
  styleUrls: ['./monitor.component.css']
})
export class MonitorComponent {
    constructor(signalRService: SignalRService,@Inject('DATA_URL') private baseUrl: string) {
        signalRService.hubConnection.on("MonitorStateUpdated", (data)=>{
            console.log('Monitor state update')
            console.log(data)
            this.monitorState = data
        })
    }
    monitorState: MonitorState | null = null
    get trackingState(): string {
        return this.monitorState ? this.monitorState.state : '-'
    }
    get detectionEnabled(): string {
        return this.monitorState ? this.monitorState.detectionEnabled.toString() : '-'
    }

    get frameRate(): string {
        return this.monitorState ? this.monitorState.frameRate.toFixed(1) : '-'
    }

    get avgContours(): string {
        return this.monitorState ? this.monitorState.avgContours.toFixed(0) : '-'
    }
    get imageAvailable(): boolean {
        return this.monitorState!=null;
    }
    get imageSrc(): string {
        return this.monitorState ? `${this.baseUrl}/Monitor/PreviewVideo` : ''
    }
}
