import { TestBed, inject } from '@angular/core/testing';

import { LandDataService } from './land-data.service';

describe('LandDataService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LandDataService]
    });
  });

  it('should be created', inject([LandDataService], (service: LandDataService) => {
    expect(service).toBeTruthy();
  }));
});
