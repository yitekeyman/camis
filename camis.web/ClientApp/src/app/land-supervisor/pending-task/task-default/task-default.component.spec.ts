import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskDefaultComponent } from './task-default.component';

describe('TaskDefaultComponent', () => {
  let component: TaskDefaultComponent;
  let fixture: ComponentFixture<TaskDefaultComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaskDefaultComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaskDefaultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
