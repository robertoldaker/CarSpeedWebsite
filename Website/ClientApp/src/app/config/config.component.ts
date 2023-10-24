import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { SignalRService } from '../signal-r/signal-r.service';
import { DataService } from '../data/data-service.service';
import { MonitorConfig, MonitorConfigArea, MonitorState } from '../data/app.data';

@Component({
  selector: 'app-config',
  templateUrl: './config.component.html',
  styleUrls: ['./config.component.css']
})
export class ConfigComponent {
    constructor(private signalRService: SignalRService,@Inject('DATA_URL') private baseUrl: string,private dataService: DataService) {
        signalRService.hubConnection.on("MonitorStateUpdated", (data)=>{
            this.monitorState = data
        })
        signalRService.hubConnection.on("MonitorConfigUpdated", ()=>{
            // Don't overwite if currently editing
            if ( !this.canEdit ) {
                this.loadMonitorConfig();
            }
        })
        this.loadMonitorConfig();
    }

    loadMonitorConfig() {
        this.dataService.GetMonitorConfig((data) => {
            this.config=data
            this.l2rDistance = data.l2r_distance.toFixed(1)
            this.r2lDistance = data.r2l_distance.toFixed(1)
            this.minSpeedImage = data.min_speed_image.toFixed(0)
            this.minSpeedSave = data.min_speed_save.toFixed(0)
            this.maxSpeedSave = data.max_speed_save.toFixed(0)
            this.fieldOfView = data.field_of_view.toFixed(0)
            this.horFlip = data.h_flip
            this.verFlip = data.v_flip
        })
    }
    monitorState: MonitorState | null = null
    config: MonitorConfig | null = null

    get imageAvailable(): boolean {
        return this.monitorState!=null;
    }

    get imageSrc(): string {
        return this.monitorState ? `${this.baseUrl}/Monitor/PreviewVideo` : ''
    }

    canEdit:boolean=false
    l2rDistance:string = '-'
    r2lDistance:string='-'
    minSpeedImage:string='-'
    minSpeedSave:string='-'
    maxSpeedSave:string='-'
    fieldOfView:string='-'
    horFlip:boolean=false
    verFlip:boolean=false            
    
    //
    edit() {
        this.canEdit=true
    }       

    save() {
        if ( this.config ) {
            if ( this.parseData() ) {
                let monitorArea:MonitorConfigArea={
                    upper_left_x: this.upperLeftX,
                    upper_left_y: this.upperLeftY,
                    lower_right_x: this.lowerRightX,
                    lower_right_y: this.lowerRightY
                }
                this.config.monitor_area = monitorArea
                console.log(monitorArea)
                this.dataService.PostMonitorConfig(this.config,()=>{})    
            }
        }
    }

    parseData():boolean {
        if ( !this.config ) {
            return false
        }
        this.errors.clear()
        //
        let res = this.parseFloat("l2rDistance",this.l2rDistance)
        if ( res.ok ) {
            this.config.l2r_distance = res.v
        }
        res = this.parseFloat("r2lDistance",this.r2lDistance)
        if ( res.ok ) {
            this.config.r2l_distance = res.v
        }
        res = this.parseInt("minSpeedImage",this.minSpeedImage)
        if ( res.ok ) {
            this.config.min_speed_image = res.v
        }
        res = this.parseInt("minSpeedSave",this.minSpeedSave)
        if ( res.ok ) {
            this.config.min_speed_save = res.v
        }
        res = this.parseInt("maxSpeedSave",this.maxSpeedSave)
        if ( res.ok ) {
            this.config.max_speed_save = res.v
        }
        res = this.parseFloat("fieldOfView",this.fieldOfView)
        if ( res.ok ) {
            this.config.field_of_view = res.v
        }
        this.config.h_flip = this.horFlip
        this.config.v_flip = this.verFlip
        //
        return this.errors.entries.length==0
    }

    errors:Map<string,string> = new Map()
    parseFloat(name:string, valueStr:string):{ok:boolean,v:number} {
        let v = parseFloat(valueStr)
        if (isNaN(v)) {
            this.errors.set(name,`Cannot parse numeric value [${valueStr}]`)
            return {ok: false, v: 0}
        } else {
            return {ok: true, v: v}
        }
    }
    parseInt(name:string, valueStr:string):{ok:boolean,v:number} {
        let v = parseInt(valueStr)
        if (isNaN(v)) {
            this.errors.set(name,`Cannot parse integer value [${valueStr}]`)
            return {ok: false, v: 0}
        } else {
            return {ok: true, v: v}
        }
    }

    getError(name: string) {
        if ( this.errors.has(name) ) {
            return this.errors.get(name)
        } else {
            return ''
        }
    }

    cancel() {
        this.canEdit = false
        this.loadMonitorConfig()
        this.startPos={x: 0, y: 0}
        this.size = {width: 0, height: 0}
    }

    inMonitorAreaEdit = false

    getMousePos(e:any):{x: number, y:number} {
        var rect = e.target.getBoundingClientRect();
        var x = e.clientX - rect.left; //x position within the element.
        var y = e.clientY - rect.top;  //y position within the element.
        return {x: x, y: y}
    }

    mouseDown(e: any) {
        this.inMonitorAreaEdit = true
        this.size = {width: 0, height: 0}
        this.startPos = this.getMousePos(e);
    }

    mouseMove(e: any) {
        this.mPos = this.getMousePos(e)
        if ( this.inMonitorAreaEdit) {
            this.size={ width: this.mPos.x - this.startPos.x, height: this.mPos.y - this.startPos.y}
        }
    }
    mouseUp(e: any) {
        this.inMonitorAreaEdit = false
    }
    mouseLeave(e: any) {
        this.inMonitorAreaEdit = false;
    }

    startPos:{x: number, y: number} = {x: 0, y: 0}
    size: {width: number, height: number} = { width: 0, height: 0}
    mPos:{x:number, y:number} = {x: 0, y: 0}

    get upperLeftX():number {
        let left=this.startPos.x;
        if ( this.size.width<0) {
            left+=this.size.width;
        }
        return left;
    }
    
    get upperLeftY():number {
        let top=this.startPos.y
        if ( this.size.height<0) {
            top+=this.size.height;
        }
        return top;
    }

    get lowerRightX():number {
        return this.upperLeftX + Math.abs(this.size.width);
    }

    get lowerRightY(): number {
        return this.upperLeftY + Math.abs(this.size.height);
    }

    get monitorAreaTop():string {
        return `${this.upperLeftY}px`
    }
    
    get monitorAreaLeft(): string {
        return `${this.upperLeftX}px`
    }

    get monitorAreaWidth():string {
        let width = Math.abs(this.size.width)
        return `${width}px`
    }

    get monitorAreaHeight():string {
        let height = Math.abs(this.size.height)
        return `${height}px`
    }
}
