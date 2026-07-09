import { Routes } from '@angular/router';
import { Login } from './features/login/login';
import { Register } from './features/register/register';
import { Home } from './features/home/home';
import { Emotions } from './features/emotions/emotions';
import { EmotionManage } from './features/emotions/emotion-manage/emotion-manage';
import { EventLogs } from './features/event-logs/event-logs';
import { EventCreate } from './features/event-logs/event-create/event-create';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'home', component: Home, canActivate: [AuthGuard] },
  { path: 'emotions', component: Emotions, canActivate: [AuthGuard] },
  { path: 'emotions/manage', component: EmotionManage, canActivate: [AuthGuard] },
  { path: 'events', component: EventLogs, canActivate: [AuthGuard] },
  { path: 'events/new', component: EventCreate, canActivate: [AuthGuard] }
];
