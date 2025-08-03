import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewLandComponent } from './new-land.component';

describe('NewLandComponent', () => {
  let component: NewLandComponent;
  let fixture: ComponentFixture<NewLandComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewLandComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewLandComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
