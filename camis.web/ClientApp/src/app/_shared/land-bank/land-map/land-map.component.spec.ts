import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LandMapComponent } from './land-map.component';

describe('LandMapComponent', () => {
  let component: LandMapComponent;
  let fixture: ComponentFixture<LandMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LandMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
