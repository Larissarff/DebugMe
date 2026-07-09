import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EventLogService } from '../../core/services/event-log.service';
import { AuthService } from '../../core/services/auth.service';
import { EventLog } from '../../core/models/event-log.model';

@Component({
  selector: 'app-event-logs',
  imports: [CommonModule, RouterModule],
  templateUrl: './event-logs.html',
  styleUrl: './event-logs.css'
})
export class EventLogs implements OnInit {
  eventLogs: EventLog[] = [];
  loading = true;
  errorMessage = '';
  deletingId: string | null = null;

  constructor(
    private eventLogService: EventLogService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadEventLogs();
  }

  private loadEventLogs(): void {
    this.loading = true;
    this.errorMessage = '';

    this.eventLogService.getAll().subscribe({
      next: (logs) => {
        this.eventLogs = logs;
        this.loading = false;
      },
      error: (error: Error) => {
        this.errorMessage = error.message;
        this.loading = false;
      }
    });
  }

  confirmDelete(id: string): void {
    const confirmed = window.confirm('Tem certeza que deseja excluir este registro?');
    if (confirmed) {
      this.deleteEvent(id);
    }
  }

  private deleteEvent(id: string): void {
    this.deletingId = id;
    this.eventLogService.delete(id).subscribe({
      next: () => {
        this.eventLogs = this.eventLogs.filter(log => log.id !== id);
        this.deletingId = null;
      },
      error: (error: Error) => {
        this.errorMessage = error.message;
        this.deletingId = null;
      }
    });
  }

  getEmotionName(log: EventLog): string {
    return log.emotion?.name || log.emotionId || 'Emoção';
  }

  getIntensityColor(intensity: number): string {
    if (intensity <= 3) return '#22c55e';
    if (intensity <= 6) return '#eab308';
    if (intensity <= 8) return '#f97316';
    return '#ef4444';
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  truncate(text: string, max: number = 100): string {
    if (!text || text.length <= max) return text || '';
    return text.substring(0, max) + '...';
  }
}
