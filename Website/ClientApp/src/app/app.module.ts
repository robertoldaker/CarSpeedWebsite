import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { DetectionsComponent } from './detections/detections.component';
import { ReportsComponent } from './reports/reports.component';
import { ShowMessageComponent } from './show-message/show-message.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { MainHeaderComponent } from './main-header/main-header.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Angular material design
import { MatSliderModule } from '@angular/material/slider';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select'
import { MatIconModule } from '@angular/material/icon'; 
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';  
import { MatButtonToggleModule } from '@angular/material/button-toggle'; 
import { MatRadioModule } from '@angular/material/radio'; 
import { MatDividerModule } from '@angular/material/divider'; 
import { MatDialogModule } from '@angular/material/dialog'; 
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressBarModule } from '@angular/material/progress-bar'; 
import { MatAutocompleteModule } from '@angular/material/autocomplete'; 
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTabsModule } from '@angular/material/tabs'; 
import { MatExpansionModule} from '@angular/material/expansion'; 
import { MatGridListModule } from '@angular/material/grid-list'; 
import { MatListModule } from '@angular/material/list'; 
import { MatTableModule } from '@angular/material/table'; 
import { MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'; 
import {MatPaginatorModule} from '@angular/material/paginator'; 


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    MainHeaderComponent,
    DetectionsComponent,
    ReportsComponent,
    ShowMessageComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'counter', component: CounterComponent },
      { path: 'fetch-data', component: FetchDataComponent },
    ]),
    BrowserAnimationsModule,
    MatSliderModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatMenuModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatRadioModule,
    MatDividerModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressBarModule,
    MatAutocompleteModule,
    MatCheckboxModule,
    MatTabsModule,
    MatExpansionModule,
    MatGridListModule,
    MatListModule,
    MatTableModule,
    MatSortModule,
    MatProgressSpinnerModule,
    MatPaginatorModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
