import { EventEmitter, Inject, Injectable } from '@angular/core';
import { DataService } from '../data/data-service.service';
import { SignalRService } from '../signal-r/signal-r.service';
import { MonitorInfo } from '../data/app.data';

@Injectable({
    providedIn: 'root'
})
export class MonitorService {

    constructor(signalRService: SignalRService,@Inject('DATA_URL') private baseUrl: string,private dataService: DataService) {
        this.monitorInfo=[]
        dataService.GetMonitorInfo((data)=>{
            this.monitorInfo = data
            // select the first connected monitor
            // of just the first one if none found
            var sm = this.monitorInfo.find(m=>m.isConnected==true);
            if ( sm ) {
                this.selectedMonitor = sm
            } else if ( this.monitorInfo.length>0 ) {
                this.selectedMonitor = this.monitorInfo[0]
            }
        })
        signalRService.hubConnection.on("MonitorConnected", (data)=>{
            let monitorName = data.monitorName
            let mi = this.monitorInfo.find(m=>m.name==monitorName)
            if (mi) {
                mi.isConnected = true
            } else {
                this.monitorInfo.push({name: monitorName, isConnected: true})
            }
        })
        signalRService.hubConnection.on("MonitorDisconnected", (data)=>{
            let monitorName = data.monitorName
            let mi = this.monitorInfo.find(m=>m.name==monitorName)
            if (mi) {
                mi.isConnected = false
            } else {
                this.monitorInfo.push({name: monitorName, isConnected: false})
            }
        })
    }

    monitorInfo: MonitorInfo[]
    private _selectedMonitor: MonitorInfo | undefined
    setSelectedMonitor(monitorName:string) {
        this._selectedMonitor = this.monitorInfo.find(m=>m.name==monitorName)
        this.SelectecMonitorChange.emit(this.selectedMonitor)
    }

    set selectedMonitor(mi: MonitorInfo | undefined) {
        this._selectedMonitor = mi;
        if ( mi) {
            this.SelectecMonitorChange.emit(this._selectedMonitor)    
        }
    }
    get selectedMonitor():MonitorInfo | undefined {
        return this._selectedMonitor
    }

    get selectedMonitorName(): string {
        return this._selectedMonitor ? this._selectedMonitor.name : ''
    }
    SelectecMonitorChange: EventEmitter<MonitorInfo> = new EventEmitter<MonitorInfo>()
} 

