import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { EventLogService } from '../../core/services/event-log.service';
import { ThemeToggleComponent } from '../../shared/components/theme-toggle/theme-toggle.component';
import { User } from '../../core/models/user.model';
import { EventLog } from '../../core/models/event-log.model';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule, ThemeToggleComponent],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit, OnDestroy {
  user: User | null = null;
  typewriterText = '';
  private fullText = 'Bem-vindo ao seu espaço';
  private charIndex = 0;
  private typewriterInterval: ReturnType<typeof setInterval> | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.user = this.authService.getCurrentUser();
    this.startTypewriter();
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

  logout(): void {
    this.authService.logout();
    window.location.href = '/login';
  }
}
