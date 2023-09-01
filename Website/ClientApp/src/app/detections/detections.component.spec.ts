import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetectionsComponent } from './detections.component';

describe('DetectionsComponent', () => {
  let component: DetectionsComponent;
  let fixture: ComponentFixture<DetectionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetectionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetectionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
