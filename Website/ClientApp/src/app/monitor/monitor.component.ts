import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { SignalRService } from '../signal-r/signal-r.service';
import { MonitorState } from '../data/app.data';

@Component({
  selector: 'app-monitor',
  templateUrl: './monitor.component.html',
  styleUrls: ['./monitor.component.css']
})
export class MonitorComponent {
    
    readonly MAX_LOG_LENGTH=60000
    readonly LOG_BUFFER=10000

    constructor(signalRService: SignalRService,@Inject('DATA_URL') private baseUrl: string) {
        signalRService.hubConnection.on("MonitorStateUpdated", (data)=>{
            this.monitorState = data
        })
        signalRService.hubConnection.on("LogMessage", (data)=>{
            this.logs += data + "\n"
            // ensure we limit the log message size
            if ( this.logs.length > this.MAX_LOG_LENGTH) {
                // add a buffer so we do not need to reduce length each time a new message appears
                this.logs = this.logs.slice(this.logs.length-this.MAX_LOG_LENGTH+this.LOG_BUFFER)
            }
            // scroll to the bottom
            if ( this.logDiv) {
                window.setTimeout(()=>{
                    if ( this.logDiv ) {
                        this.logDiv.nativeElement.scrollTop = this.logDiv.nativeElement.scrollHeight;
                    }
                }, 200)
            }
        })
    }
    
    @ViewChild('logDiv') logDiv: ElementRef | undefined;
    
    logs: string = ''
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