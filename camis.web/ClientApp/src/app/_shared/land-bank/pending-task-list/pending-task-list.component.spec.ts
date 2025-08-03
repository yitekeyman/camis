import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PendingTaskListComponent } from './pending-task-list.component';

describe('PendingTaskListComponent', () => {
  let component: PendingTaskListComponent;
  let fixture: ComponentFixture<PendingTaskListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PendingTaskListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PendingTaskListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
