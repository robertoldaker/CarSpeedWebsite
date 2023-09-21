import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetectionsTableComponent } from './detections-table.component';

describe('DetectionsTableComponent', () => {
  let component: DetectionsTableComponent;
  let fixture: ComponentFixture<DetectionsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetectionsTableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetectionsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
