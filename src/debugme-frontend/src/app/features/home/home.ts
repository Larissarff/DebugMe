import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { EventLogService } from '../../core/services/event-log.service';
import { User } from '../../core/models/user.model';
import { EventLog } from '../../core/models/event-log.model';

interface CalendarDay {
  day: number;
  date: string;
  isToday: boolean;
  isCurrentMonth: boolean;
  count: number;
  maxIntensity: number;
}

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy {
  user: User | null = null;
  typewriterText = '';
  private fullText = 'Bem-vindo ao seu espaço';
  private charIndex = 0;
  private typewriterInterval: ReturnType<typeof setInterval> | null = null;

  eventLogs: EventLog[] = [];
  calendarWeeks: CalendarDay[][] = [];
  currentMonthLabel = '';
  loadingCalendar = true;

  selectedDateEvents: EventLog[] = [];
  selectedDateLabel = '';
  popupVisible = false;

  constructor(
    private authService: AuthService,
    private eventLogService: EventLogService
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getCurrentUser();
    this.startTypewriter();
    this.loadCalendarData();
  }

  ngOnDestroy(): void {
    if (this.typewriterInterval) {
      clearInterval(this.typewriterInterval);
    }
  }

  private startTypewriter(): void {
    this.typewriterInterval = setInterval(() => {
      if (this.charIndex < this.fullText.length) {
        this.typewriterText += this.fullText.charAt(this.charIndex);
        this.charIndex++;
      } else {
        if (this.typewriterInterval) {
          clearInterval(this.typewriterInterval);
          this.typewriterInterval = null;
        }
      }
    }, 60);
  }

  private loadCalendarData(): void {
    this.eventLogService.getAll().subscribe({
      next: (logs) => {
        this.eventLogs = logs;
        this.buildCalendar();
        this.loadingCalendar = false;
      },
      error: () => {
        this.buildCalendar();
        this.loadingCalendar = false;
      }
    });
  }

  private buildCalendar(): void {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth();

    this.currentMonthLabel = now.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' });

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startOffset = firstDay.getDay();
    const daysInMonth = lastDay.getDate();

    const todayStr = now.toISOString().slice(0, 10);

    const eventCounts = new Map<string, { count: number; maxIntensity: number }>();
    for (const log of this.eventLogs) {
      const dateStr = log.eventDate?.slice(0, 10) || log.createdAt?.slice(0, 10);
      if (!dateStr) continue;
      const existing = eventCounts.get(dateStr);
      if (existing) {
        existing.count++;
        if (log.intensity > existing.maxIntensity) existing.maxIntensity = log.intensity;
      } else {
        eventCounts.set(dateStr, { count: 1, maxIntensity: log.intensity });
      }
    }

    const weeks: CalendarDay[][] = [];
    let currentWeek: CalendarDay[] = [];

    for (let i = 0; i < startOffset; i++) {
      currentWeek.push({ day: 0, date: '', isToday: false, isCurrentMonth: false, count: 0, maxIntensity: 0 });
    }

    for (let day = 1; day <= daysInMonth; day++) {
      const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
      const data = eventCounts.get(dateStr) || { count: 0, maxIntensity: 0 };

      currentWeek.push({
        day,
        date: dateStr,
        isToday: dateStr === todayStr,
        isCurrentMonth: true,
        count: data.count,
        maxIntensity: data.maxIntensity
      });

      if (currentWeek.length === 7) {
        weeks.push(currentWeek);
        currentWeek = [];
      }
    }

    if (currentWeek.length > 0) {
      while (currentWeek.length < 7) {
        currentWeek.push({ day: 0, date: '', isToday: false, isCurrentMonth: false, count: 0, maxIntensity: 0 });
      }
      weeks.push(currentWeek);
    }

    this.calendarWeeks = weeks;
  }

  getIntensityColor(intensity: number): string {
    if (intensity <= 0) return 'transparent';
    if (intensity <= 2) return '#e8d5f5';
    if (intensity <= 4) return '#c9a0e8';
    if (intensity <= 6) return '#a560d4';
    if (intensity <= 8) return '#7b3db8';
    return '#643aa4';
  }

  openDayPopup(day: CalendarDay): void {
    if (!day.isCurrentMonth || day.count === 0) return;
    this.selectedDateEvents = this.eventLogs.filter(log => {
      const dateStr = log.eventDate?.slice(0, 10) || log.createdAt?.slice(0, 10);
      return dateStr === day.date;
    });
    this.selectedDateLabel = this.formatDateLabel(day.date);
    this.popupVisible = true;
  }

  closePopup(): void {
    this.popupVisible = false;
    this.selectedDateEvents = [];
  }

  formatDateLabel(dateStr: string): string {
    const [year, month, day] = dateStr.split('-');
    const date = new Date(Number(year), Number(month) - 1, Number(day));
    return date.toLocaleDateString('pt-BR', {
      weekday: 'long',
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  formatEventTime(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
  }

  getEmotionName(log: EventLog): string {
    return log.emotion?.name || 'Emoção';
  }

  logout(): void {
    this.authService.logout();
    window.location.href = '/login';
  }
}
