import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ShowMessageService } from '../show-message/show-message.service';
import { Detection, MonitorConfig, Paged, TrackingData } from './app.data';
import { DetectionFilterImp } from '../detections/detections-table/detections-table.component';

@Injectable({
    providedIn: 'root'
})

export class DataService {
    constructor( private http: HttpClient, 
                 private showMessageService: ShowMessageService,
                 @Inject('DATA_URL') private baseUrl: string) { 
    }

    public GetDetections(onLoad: (resp: Detection[])=>void) {
        this.getRequest<Detection[]>("/Detections/All",(resp)=>{
            resp.forEach(d=>{
                d.dateTime = new Date(d.dateTime)
            })
            onLoad(resp)
        });
    }

    public GetFilteredDetections(data: DetectionFilterImp, onLoad: (resp: Paged<Detection>)=>void) {
        let params = data.getHttpParams()
        this.getRequestWithParams<Paged<Detection>>("/Detections/Filtered",params, (resp)=>{
            resp.data.forEach(d=>{
                d.dateTime = new Date(d.dateTime)
            })
            onLoad(resp)
        });
    }

    public GetTrackingData(detectionId: number, onLoad: (resp: TrackingData[])=>void) {
        let p = new HttpParams();
        p=p.append("detectionId",detectionId)
        this.getRequestWithParams<TrackingData[]>("/Detections/TrackingData",p, (resp)=>{            
            onLoad(resp)
        });
    }

    public StartMonitor() {
        this.getBasicRequest("/Monitor/StartMonitor",()=>{});
    }

    public StopMonitor() {
        this.getBasicRequest("/Monitor/StopMonitor",()=>{});
    }

    public ToggleDetection() {
        this.getBasicRequest("/Monitor/ToggleDetection",()=>{});
    }

    public ResetTracking() {
        this.getBasicRequest("/Monitor/ResetTracking",()=>{});
    }

    public GetMonitorConfig(onLoad: (resp: MonitorConfig)=>void) {
        this.getRequest<MonitorConfig>("/Monitor/Config",(data)=> {
            onLoad(data)
        });
    }

    public PostMonitorConfig(config: MonitorConfig, onOK: (resp:string )=>void) {
        this.postRequestWithMessage<MonitorConfig>("Saving","/Monitor/EditConfig",config,onOK)
    }

    /* shared */
    private getBasicRequest(url: string, onLoad: (resp: any)=>void | undefined) {
        this.http.get(this.baseUrl + url).subscribe(
        { next: (resp) => {
            if ( onLoad) {
                onLoad(resp);
            }
        }, error: (resp) => { 
            this.logErrorMessage(resp);
        }})
    }

    private getRequest<T>(url: string, onLoad: (resp: T)=>void | undefined) {
        this.http.get<T>(this.baseUrl + url).subscribe(
        { next: (resp) => {
            if ( onLoad) {
                onLoad(resp);
            }
        }, error: (resp) => { 
            this.logErrorMessage(resp);
        }})

    }

    private getRequestWithParams<T>(url: string, params: HttpParams, onLoad: (resp: T)=>void | undefined) {
        this.http.get<T>(this.baseUrl + url, {params: params}).subscribe(
        { next: (resp) => {
            if ( onLoad) {
                onLoad(resp);
            }
        }, error: (resp) => { 
            this.logErrorMessage(resp);
        }})

    }

    private getRequestWithMessage<T>(message: string, url: string, onLoad: (resp: T)=>void | undefined) {
        this.showMessageService.showMessage(message);
        this.http.get<T>(this.baseUrl + url).subscribe(
        { next: (resp) => {
            this.showMessageService.clearMessage();
            if ( onLoad) {
                onLoad(resp);
            }
        }, error: (resp) => { 
            this.showMessageService.clearMessage();
            this.logErrorMessage(resp);
        }})
    }

    private postRequest<T>(url: string, data: T,onOk: (resp: string)=>void | undefined) {
        this.http.post<string>(this.baseUrl + url, data).subscribe(
        { next: (resp) => {
            if ( onOk) {
                onOk(resp);
            }
        }, error: (resp) => { 
            this.logErrorMessage(resp);
        }})    
    }

    private postRequestWithMessage<T>(message: string, url: string, data: T,onOk: (resp: string)=>void | undefined) {
        this.showMessageService.showMessage(message);
        this.http.post<string>(this.baseUrl + url, data).subscribe(
        { next: (resp) => {
            this.showMessageService.clearMessage();
            if ( onOk) {
                onOk(resp);
            }
        }, error: (resp) => { 
            this.showMessageService.clearMessage();
            this.logErrorMessage(resp);
        }})    
    }

    private postDialogRequest<T>(url: string, data: T,onOk: (resp: string)=>void | undefined, onError: (error: any)=> void | undefined) {
        this.http.post<string>(this.baseUrl + url, data).subscribe(
        { next: (resp) => {
            if ( onOk) {
                onOk(resp);
            }
        }, error: (resp) => { 
            if ( onError && resp.status == 422) { 
                onError(resp.error) 
            } else {
                this.logErrorMessage(resp);
            }
        }})
    }

    private logErrorMessage(error:any) {
        let message:string = error.message;
        if ( typeof error.error === 'string') {
            message += '\n\n' + error.error;
        }        
        this.showMessageService.showModalErrorMessage(message)
    }

}
