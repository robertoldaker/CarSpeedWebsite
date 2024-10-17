import { Component } from '@angular/core';
import { DataService } from '../data/data-service.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent {

    constructor(private dataService: DataService) {

    }

    backupDbLocally() {
        console.log('backup db locally!')
        this.dataService.BackupDbLocally()
    }
}
