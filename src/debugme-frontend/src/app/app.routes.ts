import { Routes } from '@angular/router';
import { Login } from './features/login/login';
import { Register } from './features/register/register';
import { Home } from './features/home/home';
import { EmotionManage } from './features/emotions/emotion-manage/emotion-manage';
import { EventLogs } from './features/event-logs/event-logs';
import { EventCreate } from './features/event-logs/event-create/event-create';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login, data: { animation: 'login' } },
  { path: 'register', component: Register, data: { animation: 'register' } },
  { path: 'home', component: Home, canActivate: [AuthGuard], data: { animation: 'home' } },
  { path: 'emotions', redirectTo: 'emotions/manage', pathMatch: 'full' },
  { path: 'emotions/manage', component: EmotionManage, canActivate: [AuthGuard], data: { animation: 'emotionManage' } },
  { path: 'events', component: EventLogs, canActivate: [AuthGuard], data: { animation: 'eventLogs' } },
  { path: 'events/new', component: EventCreate, canActivate: [AuthGuard], data: { animation: 'eventCreate' } }
];
