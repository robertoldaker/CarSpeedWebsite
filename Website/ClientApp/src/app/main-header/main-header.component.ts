import { Component } from '@angular/core';
import { MonitorService } from '../monitor/monitor.service';

@Component({
  selector: 'app-main-header',
  templateUrl: './main-header.component.html',
  styleUrls: ['./main-header.component.css']
})
export class MainHeaderComponent {

    constructor(public monitorService: MonitorService) {
        
    }

    showAboutDialog() {

    }

    mode = 'Development'
    title = "Car Speed v1.0"

    selectedMonitorChanged(e: any) {
        let monitorName = e.target.value
        this.monitorService.setSelectedMonitor(monitorName)
    }
}
