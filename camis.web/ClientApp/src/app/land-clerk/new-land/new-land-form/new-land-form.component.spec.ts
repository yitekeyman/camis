import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewLandFormComponent } from './new-land-form.component';

describe('NewLandFormComponent', () => {
  let component: NewLandFormComponent;
  let fixture: ComponentFixture<NewLandFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewLandFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewLandFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
