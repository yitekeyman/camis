import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LandDashboardComponent } from './land-dashboard.component';

describe('LandDashboardComponent', () => {
  let component: LandDashboardComponent;
  let fixture: ComponentFixture<LandDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LandDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
