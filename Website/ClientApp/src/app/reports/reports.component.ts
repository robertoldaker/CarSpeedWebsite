import { Component } from '@angular/core';
import { EChartsOption } from 'echarts';
import { DataService } from '../data/data-service.service';
import { DetectionGroupsImp } from '../data/app.data';

@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.css']
})
export class ReportsComponent {
    constructor(private dataService: DataService) {
        this.detectionGroups = new DetectionGroupsImp()
        this.detectionGroups.maxSd = 3
        this.detectionGroups.speedLimits = [10,20,30,40,50]
    }
    
    detectionGroups: DetectionGroupsImp

    groupData:number[] = []
    chartSelect: ChartSelect = ChartSelect.NUMBER_OF_DETECTIONS
    chartSelections: {id: number, name: string}[] = [{id: ChartSelect.PERCENT, name: 'Percent'},{id: ChartSelect.NUMBER_OF_DETECTIONS, name: 'Number of detections'}]
    chartInstance: any
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
        yAxis: {
            name: 'Number of detections',
            type: 'value',
            nameTextStyle: {
                fontWeight: 'bold',
                fontSize: 14
            }
        },
        series: {
            data: this.groupData, type: 'bar'
        }
    };

    loadData() {
        this.dataService.GetDetectionGroupData(this.detectionGroups,(results)=>{
            this.groupData=results;
            this.fillChartOptions()
        })
    }

    fillChartOptions() {
        if ( this.chartSelect == ChartSelect.NUMBER_OF_DETECTIONS) {
            this.chartOptions.series = {data: this.groupData, type: 'bar'}
        } else if ( this.chartSelect == ChartSelect.PERCENT) {
            let groupData = Array.from(this.groupData)
            let total =0;
            this.groupData.forEach(m=>total+=m)
            //
            for (let i=0;i<groupData.length;i++) {
                groupData[i] = (groupData[i]/total)*100
            }
            this.chartOptions.series = {data: groupData, type: 'bar'}
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
