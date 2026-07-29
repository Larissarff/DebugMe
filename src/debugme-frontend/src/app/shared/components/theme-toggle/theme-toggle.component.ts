import { Component, inject } from '@angular/core';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  template: `
    <button class="theme-toggle" (click)="themeService.toggle()" [attr.aria-label]="label()">
      <span class="toggle-icon">{{ icon() }}</span>
    </button>
  `,
  styles: [`
    .theme-toggle {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 40px;
      height: 40px;
      border: 1.5px solid rgba(165, 96, 212, 0.2);
      border-radius: 14px;
      background: rgba(255, 255, 255, 0.5);
      backdrop-filter: blur(10px);
      cursor: pointer;
      transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
      padding: 0;
      font-size: 18px;
      line-height: 1;
    }

    .theme-toggle:hover {
      background: rgba(165, 96, 212, 0.1);
      border-color: rgba(165, 96, 212, 0.4);
      transform: scale(1.08);
      box-shadow: 0 4px 12px rgba(165, 96, 212, 0.2);
    }

    :host-context([data-theme="dark"]) .theme-toggle {
      background: rgba(30, 20, 50, 0.5);
      border-color: rgba(165, 96, 212, 0.25);
    }

    :host-context([data-theme="dark"]) .theme-toggle:hover {
      background: rgba(165, 96, 212, 0.15);
      border-color: rgba(165, 96, 212, 0.5);
    }

    .toggle-icon {
      transition: transform 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
      display: inline-block;
    }

    .theme-toggle:hover .toggle-icon {
      transform: rotate(15deg);
    }
  `]
})
export class ThemeToggleComponent {
  themeService = inject(ThemeService);

  icon(): string {
    return this.themeService.theme() === 'dark' ? '☀️' : '🌙';
  }

  label(): string {
    return this.themeService.theme() === 'dark' ? 'Modo claro' : 'Modo escuro';
  }
}
