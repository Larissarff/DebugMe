import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Emotions } from './emotions';

describe('Emotions', () => {
  let component: Emotions;
  let fixture: ComponentFixture<Emotions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Emotions],
    }).compileComponents();

    fixture = TestBed.createComponent(Emotions);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
