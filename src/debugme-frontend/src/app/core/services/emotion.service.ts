import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Emotion } from '../models/emotion.model';

@Injectable({
  providedIn: 'root'
})
export class EmotionService {
  private readonly baseUrl: string = 'http://localhost:5165/api/emotion';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Emotion[]> {
    return this.http.get<Emotion[]>(this.baseUrl);
  }
}