import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ColData, ColDataValueType } from '../filter-table-header/filter-table-header.component';
@Component({
  selector: 'app-filter-table-numeric-dialog',
  templateUrl: './filter-table-numeric-dialog.component.html',
  styleUrls: ['./filter-table-numeric-dialog.component.css']
})
export class FilterTableNumericDialogComponent {
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
        if ( this.data.isValid()) {
            this.data.isActive = true;
            this.dialogRef.close(this.data);    
        } else {
            this.data.isActive = false;
            this.dialogRef.close(null);    
        }
    }
    valueType = ColDataValueType
}
