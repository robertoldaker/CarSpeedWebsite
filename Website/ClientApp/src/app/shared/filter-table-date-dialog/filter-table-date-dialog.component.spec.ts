import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterTableDateDialogComponent } from './filter-table-date-dialog.component';

describe('FilterTableDateDialogComponent', () => {
  let component: FilterTableDateDialogComponent;
  let fixture: ComponentFixture<FilterTableDateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilterTableDateDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilterTableDateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
