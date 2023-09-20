import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ColData, ColDataValueType } from '../filter-table-header/filter-table-header.component';
import { FilterTableNumericDialogComponent } from '../filter-table-numeric-dialog/filter-table-numeric-dialog.component';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-filter-table-date-dialog',
  templateUrl: './filter-table-date-dialog.component.html',
  styleUrls: ['./filter-table-date-dialog.component.css']
})
export class FilterTableDateDialogComponent {
    constructor(public dialogRef: MatDialogRef<FilterTableNumericDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: ColData)
    {
    }
    typeChanged(e: any) {
        if ( e.value ) {
            this.data.columnFilter.type = e.value;
        }
    }
    isChecked(valueType: ColDataValueType):boolean {
        return valueType==this.data.columnFilter.type
    }
    filter() {
        if ( this.data.isValid() ) {
            this.data.isActive = true;
            this.dialogRef.close(this.data);            
        } else {
            this.dialogRef.close(null);            
        }
    }
    valueType = ColDataValueType
}
