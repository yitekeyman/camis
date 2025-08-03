import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewLandDefaultComponent } from './new-land-default.component';

describe('NewLandDefaultComponent', () => {
  let component: NewLandDefaultComponent;
  let fixture: ComponentFixture<NewLandDefaultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewLandDefaultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewLandDefaultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
