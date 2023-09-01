import { Component } from '@angular/core';

@Component({
  selector: 'app-main-header',
  templateUrl: './main-header.component.html',
  styleUrls: ['./main-header.component.css']
})
export class MainHeaderComponent {

    showAboutDialog() {

    }

    mode = 'Development'
    title = "Car Speed v1.0"
}
