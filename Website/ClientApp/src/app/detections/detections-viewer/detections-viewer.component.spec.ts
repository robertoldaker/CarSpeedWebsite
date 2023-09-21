import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetectionsViewerComponent } from './detections-viewer.component';

describe('DetectionsViewerComponent', () => {
  let component: DetectionsViewerComponent;
  let fixture: ComponentFixture<DetectionsViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetectionsViewerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetectionsViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
