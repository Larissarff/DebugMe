import { Routes } from '@angular/router';
import { Login } from './features/login/login';
import { Home } from './features/home/home';
import { Emotions } from './features/emotions/emotions';
import { EventLogs } from './features/event-logs/event-logs';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'home', component: Home },
  { path: 'emotions', component: Emotions },
  { path: 'event-logs', component: EventLogs }
];