import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventLogs } from './event-logs';

describe('EventLogs', () => {
  let component: EventLogs;
  let fixture: ComponentFixture<EventLogs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventLogs],
    }).compileComponents();

    fixture = TestBed.createComponent(EventLogs);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
