import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailLandMapComponent } from './detail-land-map.component';

describe('DetailLandMapComponent', () => {
  let component: DetailLandMapComponent;
  let fixture: ComponentFixture<DetailLandMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetailLandMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailLandMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
