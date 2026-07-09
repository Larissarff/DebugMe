import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Emotion } from '../models/emotion.model';

export interface CreateEmotionRequest {
  name: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmotionService {
  constructor(private api: ApiService) {}

  getAll(): Observable<Emotion[]> {
    return this.api.get<Emotion[]>('/api/emotion/all');
  }

  getById(id: string): Observable<Emotion> {
    return this.api.get<Emotion>(`/api/emotion/id/${id}`);
  }

  getByName(name: string): Observable<Emotion> {
    return this.api.get<Emotion>(`/api/emotion/name/${name}`);
  }

  create(data: CreateEmotionRequest): Observable<Emotion> {
    return this.api.post<Emotion>('/api/emotion/create', data);
  }
}
