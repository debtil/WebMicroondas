import { TestBed } from '@angular/core/testing';

import { MicrowaveService } from './microwave.service';

describe('MicrowaveService', () => {
  let service: MicrowaveService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MicrowaveService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
