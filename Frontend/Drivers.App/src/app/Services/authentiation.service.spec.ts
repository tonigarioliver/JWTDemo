import { TestBed } from '@angular/core/testing';

import { AuthentiationService } from './authentiation.service';

describe('AuthentiationService', () => {
  let service: AuthentiationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthentiationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
