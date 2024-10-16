import { Component } from '@angular/core';
import { EChartsOption } from 'echarts';
import { DataService } from '../data/data-service.service';
import { DetectionGroupsImp } from '../data/app.data';
import { SignalRService } from '../signal-r/signal-r.service';
import { MonitorService } from '../monitor/monitor.service';

@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.css']
})
export class ReportsComponent {
    constructor(private dataService: DataService, private signalRService: SignalRService, private monitorService: MonitorService) {
        this.detectionGroups = new DetectionGroupsImp()
        this.detectionGroups.maxSd = 3
        this.detectionGroups.speedLimits = [10,20,30,40,50]

        this.signalRService.hubConnection.on("NewDetectionLoaded", (data) => {
            // reload data if its for the selected monitor
            if ( data.monitorName===this.monitorService.selectedMonitorName) {
                this.loadData()
            }
        })
    }
    
    detectionGroups: DetectionGroupsImp

    groupData:number[] = []
    totalDetections: number = 0
    percentAbove20:number = 0
    chartSelect: ChartSelect = ChartSelect.NUMBER_OF_DETECTIONS
    chartSelections: {id: number, name: string}[] = [{id: ChartSelect.PERCENT, name: 'Percent'},{id: ChartSelect.NUMBER_OF_DETECTIONS, name: 'Number of detections'}]
    chartInstance: any
    yAxis:any = {
        name: '',
        type: 'value',
        nameTextStyle: {
            fontWeight: 'bold',
            fontSize: 14
        }
    }
    series:any = {
        data: this.groupData, 
        type: 'bar'        
    }
    chartOptions: EChartsOption = {
        title: {
            text: 'Coronation Avenue vehicle speeds 2024',
            textAlign: 'center',
            left: '50%',
            padding: 0
        },
        xAxis: {
            type: 'category',
            data: ['<10', '10-20', '20-30', '30-40', '40-50', '>50'],
            name: 'Speed (mph)',
            nameLocation: 'middle',
            nameGap: 30,
            nameTextStyle: {
                fontWeight: 'bold',
                fontSize: 14
            }
        },
        yAxis: this.yAxis,
        tooltip: {
            trigger: 'axis',
            valueFormatter: (value) => {
                let str = ''
                if ( typeof(value) === 'number') {
                    str = value.toFixed(0)
                } 
                return str;
            }
        },
        series: this.series
    };

    loadData() {
        this.dataService.GetDetectionGroupData(this.detectionGroups,(results)=>{
            this.groupData=results;
            this.totalDetections=0;
            this.groupData.forEach(m=>this.totalDetections+=m)
            let totalAbove20 = this.groupData[2] + this.groupData[3] + this.groupData[4] +this.groupData[5]
            this.percentAbove20 = (totalAbove20/this.totalDetections) * 100
            this.fillChartOptions()
        })
    }

    fillChartOptions() {
        if ( this.chartSelect == ChartSelect.NUMBER_OF_DETECTIONS) {
            this.chartOptions.series = {data: this.groupData, type: 'bar'}
            this.yAxis.name = 'Number of detections'
        } else if ( this.chartSelect == ChartSelect.PERCENT) {
            let groupData = Array.from(this.groupData)
            let total =0;
            this.groupData.forEach(m=>total+=m)
            //
            for (let i=0;i<groupData.length;i++) {
                groupData[i] = (groupData[i]/total)*100
            }
            this.chartOptions.series = {data: groupData, type: 'bar'}
            this.yAxis.name = 'Percentage'
        } else {
            throw `Unexpected chartSelect value ${this.chartSelect}`
        }
        if ( this.chartInstance ) {
            this.chartInstance.setOption(this.chartOptions)
        }
    }

    chartSelectChanged(e: any) {
        this.chartSelect = e.target.value
        this.fillChartOptions()
    }

    onChartInit(e: any) {
        this.chartInstance = e
        this.loadData()
    }
}

export enum ChartSelect { PERCENT, NUMBER_OF_DETECTIONS }
