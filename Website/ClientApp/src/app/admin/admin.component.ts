import { Component } from '@angular/core';
import { DataService } from '../data/data-service.service';
import { DataModel } from '../data/app.data';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {

    constructor(private dataService: DataService) {
        this.refresh()
    }

    runningPurge = false;
    message = ''
    inPurge = false;
    inCleanup = false;
    model: DataModel | undefined;

    backupDbLocally() {
        console.log('backup db locally!')
        this.dataService.BackupDbLocally()
    }

    purge() {
        console.log('purging')
        this.inPurge = true;
        this.dataService.Purge( (resp)=>{
            this.message = resp.mess
            this.inPurge = false
            console.log(resp)
        });
    }

    cleanup() {
        this.inCleanup = true;
        this.dataService.PerformCleanup( (result)=>{
            this.inCleanup = false;
            this.refresh()
        });
    }

    refresh() {
        this.dataService.GetDataModel((dm)=>{
            this.model = dm;
        });
    }
}
