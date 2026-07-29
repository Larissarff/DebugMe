import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EventLogService } from '../../core/services/event-log.service';
import { AuthService } from '../../core/services/auth.service';
import { EventLog } from '../../core/models/event-log.model';
import { ToastService } from '../../shared/components/toast/toast.service';

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

  sparklinePath = '';
  sparklineViewBox = '0 0 200 40';
  avgIntensity = 0;
  trendLabel = '';

  constructor(
    private eventLogService: EventLogService,
    private authService: AuthService
  ) {}

  private toastService = inject(ToastService);

  ngOnInit(): void {
    console.log('[EventLogs] ngOnInit called');
    this.loadEventLogs();
  }

  private loadEventLogs(): void {
    console.log('[EventLogs] loadEventLogs called');
    this.loading = true;
    this.errorMessage = '';
    this.eventLogService.getAll().subscribe({
      next: (logs) => {
        console.log('[EventLogs] logs loaded:', logs.length);
        this.eventLogs = logs;
        this.loading = false;
        this.generateSparkline();
      },
      error: (error: Error) => {
        console.error('[EventLogs] error:', error);
        this.errorMessage = error.message;
        this.loading = false;
      }
    });
  }

  private generateSparkline(): void {
    const logs = [...this.eventLogs].reverse();
    if (logs.length < 2) return;

    const intensities = logs.map(l => l.intensity);
    const max = 10;
    const min = 1;
    const width = 200;
    const height = 40;
    const padding = 4;

    const xStep = (width - padding * 2) / (intensities.length - 1);
    const points = intensities.map((val, i) => {
      const x = padding + i * xStep;
      const y = height - padding - ((val - min) / (max - min)) * (height - padding * 2);
      return `${x},${y}`;
    });

    this.sparklinePath = `M${points.join(' L')}`;
    this.sparklineViewBox = `0 0 ${width} ${height}`;

    const sum = intensities.reduce((a, b) => a + b, 0);
    this.avgIntensity = Math.round((sum / intensities.length) * 10) / 10;

    const first = intensities[0];
    const last = intensities[intensities.length - 1];
    if (last > first) this.trendLabel = '↑ crescendo';
    else if (last < first) this.trendLabel = '↓ diminuindo';
    else this.trendLabel = '→ estável';
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
        this.generateSparkline();
        this.toastService.success('Registro excluído com sucesso');
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
