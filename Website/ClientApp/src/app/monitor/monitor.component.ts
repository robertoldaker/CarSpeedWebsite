import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { SignalRService } from '../signal-r/signal-r.service';
import { MonitorInfo, MonitorState } from '../data/app.data';
import { DataService } from '../data/data-service.service';
import { MonitorService } from './monitor.service';

@Component({
  selector: 'app-monitor',
  templateUrl: './monitor.component.html',
  styleUrls: ['./monitor.component.css']
})
export class MonitorComponent {
    
    readonly MAX_LOG_LENGTH=60000
    readonly LOG_BUFFER=10000

    constructor(signalRService: SignalRService,@Inject('DATA_URL') private baseUrl: string,private dataService: DataService, private monitorService: MonitorService) {

        monitorService.SelectecMonitorChange.subscribe((data)=>{
            this.currentMonitorInfo = data
            this.currentMonitorName = data.name;
            this.monitorState = this.monitorStates.get(data.name)
            let log = this.logs.get(data.name)
            this.log = log ? log : ''    
        })

        signalRService.hubConnection.on("MonitorStateUpdated", (data)=>{
            let monitorName = data.monitorName
            this.monitorStates.set(monitorName,data.monitorState)
            if ( monitorName===this.currentMonitorName) {
                this.monitorState = data.monitorState
            }
        })

        signalRService.hubConnection.on("LogMessage", (data)=>{

            let monitorName = data.monitorName
            let log = this.logs.get(monitorName)
            if (!log) {
                log = ''
            }
            log += data.message + "\n"
            // ensure we limit the log message size
            if ( log.length > this.MAX_LOG_LENGTH) {
                // add a buffer so we do not need to reduce length each time a new message appears
                log = log.slice(log.length-this.MAX_LOG_LENGTH+this.LOG_BUFFER)
            }
            this.logs.set(monitorName,log)
            if ( this.currentMonitorName === monitorName  ) {
                this.log = log
                this.scrollLogToBottom()
            }
        })
    }
    
    @ViewChild('logDiv') logDiv: ElementRef | undefined;
    
    currentMonitorName: string = ''
    currentMonitorInfo: MonitorInfo | undefined
    monitorStates: Map<string,MonitorState> = new Map()
    monitorState: MonitorState | undefined
    logs: Map<string,string> = new Map()
    log: string = ''

    addToLog(log:string|undefined, message:string):string {
        if (!log) {
            log = ''
        }
        log += message + "\n"
        // ensure we limit the log message size
        if ( log.length > this.MAX_LOG_LENGTH) {
            // add a buffer so we do not need to reduce length each time a new message appears
            log = log.slice(log.length-this.MAX_LOG_LENGTH+this.LOG_BUFFER)
        }
        return log
    }

    scrollLogToBottom() {
        // scroll to the bottom
        if ( this.logDiv) {
            window.setTimeout(()=>{
                if ( this.logDiv ) {
                    this.logDiv.nativeElement.scrollTop = this.logDiv.nativeElement.scrollHeight;
                }
            }, 200)
        }
    }
    
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

    get lightLevel(): string {
        return this.monitorState ? this.monitorState.lightLevel.toFixed(0) : '-'
    }

    get cpu(): string {
        return this.monitorState ? this.monitorState.cpu.toFixed(0) : '-'
    }

    get imageAvailable(): boolean {
        return this.monitorState!=null;
    }

    get imageSrc(): string {
        return this.monitorState ? `${this.baseUrl}/Monitor/PreviewVideo?monitorName=${encodeURI(this.currentMonitorName)}` : ''
    }

    get monitoringText():string {
        return this.isRunning() ? "Stop monitoring" : "Start monitoring"
    }
    get detectionText():string {
        return this.isDetecting() ? "Stop detection" : "Start detection"
    }
    isRunning() {
        return this.monitorState && this.monitorState.state!=null && this.monitorState.state!="" && this.monitorState.state!="IDLE"
    }
    isConnected() {
        return this.currentMonitorInfo && this.currentMonitorInfo.isConnected
    }
    isDetecting() {
        return this.monitorState && this.monitorState.detectionEnabled
    }
    startMonitoring() {
        if (this.currentMonitorName) {
            this.dataService.StartMonitor(this.currentMonitorName);
        }
    }
    stopMonitoring() {
        if (this.currentMonitorName) {
            this.dataService.StopMonitor(this.currentMonitorName);
        }
    }
    toggleDetection() {
        if ( this.currentMonitorName) {
            this.dataService.ToggleDetection(this.currentMonitorName);
        }
    }
    resetTracking() {
        if ( this.currentMonitorName) {
            this.dataService.ResetTracking(this.currentMonitorName);
        }
    }

    reboot() {

    }

    shutdown() {

    }
}
