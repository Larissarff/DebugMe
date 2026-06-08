import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { EventLog } from '../models/event-log.model';

export interface CreateEventLogRequest {
  emotionId: string;
  intensity: number;
  description: string;
  eventDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class EventLogService {
  constructor(private api: ApiService) {}

  getAll(): Observable<EventLog[]> {
    return this.api.get<EventLog[]>('/api/eventlog/all');
  }

  getById(id: string): Observable<EventLog> {
    return this.api.get<EventLog>(`/api/eventlog/${id}`);
  }

  create(data: CreateEventLogRequest): Observable<EventLog> {
    return this.api.post<EventLog>('/api/eventlog/create', data);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`/api/eventlog/${id}`);
  }
}
