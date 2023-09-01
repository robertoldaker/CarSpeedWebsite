import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ShowMessageService } from '../show-message/show-message.service';
import { Detection } from './app.data';

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
