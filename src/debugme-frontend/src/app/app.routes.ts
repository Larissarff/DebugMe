import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { EmotionsComponent } from './features/emotions/emotions.component';
import { EventLogsComponent } from './features/event-logs/event-logs.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'emotions', component: EmotionsComponent },
  { path: 'event-logs', component: EventLogsComponent }
];