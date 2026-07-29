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

  it('should return lilac for low intensity', () => {
    expect(component.getIntensityColor(2)).toBe('#e8d5f5');
  });

  it('should return medium purple for medium intensity', () => {
    expect(component.getIntensityColor(5)).toBe('#a560d4');
  });

  it('should return dark purple for high intensity', () => {
    expect(component.getIntensityColor(8)).toBe('#7b3db8');
  });

  it('should return deepest purple for very high intensity', () => {
    expect(component.getIntensityColor(10)).toBe('#643aa4');
  });
});
