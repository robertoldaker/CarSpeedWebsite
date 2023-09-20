import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterTableHeaderComponent } from './filter-table-header.component';

describe('FilterTableHeaderComponent', () => {
  let component: FilterTableHeaderComponent;
  let fixture: ComponentFixture<FilterTableHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FilterTableHeaderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilterTableHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
