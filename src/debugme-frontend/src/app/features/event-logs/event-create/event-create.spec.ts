import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EventCreate } from './event-create';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('EventCreate', () => {
  let component: EventCreate;
  let fixture: ComponentFixture<EventCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventCreate],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(EventCreate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form when emotion not selected', () => {
    expect(component.eventForm.valid).toBeFalsy();
  });

  it('should validate emotionId as required', () => {
    const emotionId = component.eventForm.controls['emotionId'];
    expect(emotionId.valid).toBeFalsy();
    emotionId.setValue('some-emotion-id');
    expect(emotionId.valid).toBeTruthy();
  });

  it('should have default intensity of 5', () => {
    expect(component.eventForm.controls['intensity'].value).toBe(5);
  });

  it('should validate intensity range', () => {
    const intensity = component.eventForm.controls['intensity'];
    intensity.setValue(0);
    expect(intensity.valid).toBeFalsy();
    intensity.setValue(11);
    expect(intensity.valid).toBeFalsy();
    intensity.setValue(5);
    expect(intensity.valid).toBeTruthy();
  });

  it('should validate description max length', () => {
    const description = component.eventForm.controls['description'];
    description.setValue('a'.repeat(501));
    expect(description.valid).toBeFalsy();
    description.setValue('a'.repeat(500));
    expect(description.valid).toBeTruthy();
  });

  it('should mark submitted on submit', () => {
    component.onSubmit();
    expect(component.submitted).toBeTruthy();
  });

  it('should start with loadingEmotions true', () => {
    expect(component.loadingEmotions).toBeTruthy();
  });

  it('should have empty emotions array initially', () => {
    expect(component.emotions).toEqual([]);
  });
});
