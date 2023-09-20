import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterTableNumericDialogComponent } from './filter-table-numeric-dialog.component';

describe('FilterTableNumericDialogComponent', () => {
  let component: FilterTableNumericDialogComponent;
  let fixture: ComponentFixture<FilterTableNumericDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilterTableNumericDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilterTableNumericDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
