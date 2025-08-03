import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchLandComponent } from './search-land.component';

describe('SearchLandComponent', () => {
  let component: SearchLandComponent;
  let fixture: ComponentFixture<SearchLandComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SearchLandComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchLandComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
