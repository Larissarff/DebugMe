import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EventLogs } from './event-logs';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('EventLogs', () => {
  let component: EventLogs;
  let fixture: ComponentFixture<EventLogs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventLogs],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(EventLogs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start with loading true', () => {
    expect(component.loading).toBeTruthy();
  });

  it('should have empty event logs initially', () => {
    expect(component.eventLogs).toEqual([]);
  });

  it('should return green color for low intensity', () => {
    expect(component.getIntensityColor(3)).toBe('#22c55e');
  });

  it('should return yellow color for medium intensity', () => {
    expect(component.getIntensityColor(5)).toBe('#eab308');
  });

  it('should return orange color for high intensity', () => {
    expect(component.getIntensityColor(7)).toBe('#f97316');
  });

  it('should return red color for very high intensity', () => {
    expect(component.getIntensityColor(9)).toBe('#ef4444');
  });
});
