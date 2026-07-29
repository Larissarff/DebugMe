import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      @for (toast of toastService.toasts$ | async; track toast.id) {
        <div class="toast toast-{{ toast.type }}">
          <span class="toast-icon">{{ icon(toast.type) }}</span>
          <span class="toast-text">{{ toast.text }}</span>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 24px;
      right: 24px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 10px;
      pointer-events: none;
    }

    .toast {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 14px 20px;
      border-radius: 16px;
      font-size: 14px;
      font-weight: 600;
      font-family: 'Inter', sans-serif;
      backdrop-filter: blur(20px) saturate(180%);
      -webkit-backdrop-filter: blur(20px) saturate(180%);
      box-shadow: 0 8px 28px rgba(0, 0, 0, 0.1);
      pointer-events: auto;
      animation: toastSlideIn 0.35s cubic-bezier(0.34, 1.56, 0.64, 1) both;
      max-width: 380px;
    }

    .toast-success {
      background: rgba(34, 197, 94, 0.12);
      border: 1px solid rgba(34, 197, 94, 0.25);
      color: #15803d;
    }

    .toast-error {
      background: rgba(239, 68, 68, 0.12);
      border: 1px solid rgba(239, 68, 68, 0.25);
      color: #b91c1c;
    }

    .toast-info {
      background: rgba(165, 96, 212, 0.12);
      border: 1px solid rgba(165, 96, 212, 0.25);
      color: #643aa4;
    }

    .toast-icon {
      font-size: 18px;
      flex-shrink: 0;
    }

    .toast-text {
      line-height: 1.4;
    }

    @keyframes toastSlideIn {
      from {
        opacity: 0;
        transform: translateX(40px) scale(0.95);
      }
      to {
        opacity: 1;
        transform: translateX(0) scale(1);
      }
    }
  `]
})
export class ToastComponent {
  toastService = inject(ToastService);

  icon(type: string): string {
    switch (type) {
      case 'success': return '✓';
      case 'error': return '✕';
      case 'info': return '→';
      default: return '';
    }
  }
}
