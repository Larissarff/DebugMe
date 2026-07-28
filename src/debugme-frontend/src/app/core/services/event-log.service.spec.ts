import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { EventLogService } from './event-log.service';
import { EventLog } from '../models/event-log.model';

describe('EventLogService', () => {
  let service: EventLogService;
  let httpMock: HttpTestingController;

  const mockEventLog: EventLog = {
    id: 'event-123',
    userId: 'user-123',
    emotionId: 'emotion-123',
    description: 'Test event',
    intensity: 7,
    eventDate: '2026-07-28T15:00:00Z',
    createdAt: '2026-07-28T15:00:00Z',
    emotion: { id: 'emotion-123', name: 'Alegria' },
    user: { id: 'user-123', name: 'Test User', email: 'test@example.com' },
  };

  beforeEach(() => {
    localStorage.setItem('debugme_token', 'mock-token');

    TestBed.configureTestingModule({
      providers: [EventLogService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(EventLogService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all event logs', () => {
    service.getAll().subscribe((logs) => {
      expect(logs).toEqual([mockEventLog]);
    });

    const req = httpMock.expectOne('http://localhost:5165/api/eventlog/all');
    expect(req.request.method).toBe('GET');
    req.flush([mockEventLog]);
  });

  it('should get event log by id', () => {
    service.getById('event-123').subscribe((log) => {
      expect(log.id).toBe('event-123');
    });

    const req = httpMock.expectOne('http://localhost:5165/api/eventlog/id/event-123');
    expect(req.request.method).toBe('GET');
    req.flush(mockEventLog);
  });

  it('should create event log', () => {
    const createRequest = {
      userId: 'user-123',
      emotionId: 'emotion-123',
      intensity: 5,
      description: 'New event',
      eventDate: '2026-07-28T15:00:00Z',
    };

    service.create(createRequest).subscribe((log) => {
      expect(log.id).toBe('event-123');
    });

    const req = httpMock.expectOne('http://localhost:5165/api/eventlog/create');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(mockEventLog);
  });

  it('should delete event log', () => {
    service.delete('event-123').subscribe();

    const req = httpMock.expectOne('http://localhost:5165/api/eventlog/delete/event-123');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should send auth header', () => {
    localStorage.setItem('debugme_token', 'my-test-token');

    service.getAll().subscribe();

    const req = httpMock.expectOne('http://localhost:5165/api/eventlog/all');
    expect(req.request.headers.get('Authorization')).toBe('Bearer my-test-token');
    req.flush([]);
  });
});
