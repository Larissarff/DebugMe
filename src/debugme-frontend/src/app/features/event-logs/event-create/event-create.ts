import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { EmotionService } from '../../../core/services/emotion.service';
import { EventLogService } from '../../../core/services/event-log.service';
import { AuthService } from '../../../core/services/auth.service';
import { Emotion } from '../../../core/models/emotion.model';

@Component({
  selector: 'app-event-create',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './event-create.html',
  styleUrl: './event-create.css'
})
export class EventCreate implements OnInit {
  eventForm: FormGroup;
  submitted = false;
  loading = false;
  loadingEmotions = true;
  errorMessage = '';
  emotions: Emotion[] = [];

  constructor(
    private formBuilder: FormBuilder,
    private emotionService: EmotionService,
    private eventLogService: EventLogService,
    private authService: AuthService,
    private router: Router
  ) {
    this.eventForm = this.formBuilder.group({
      emotionId: ['', Validators.required],
      intensity: [5, [Validators.required, Validators.min(1), Validators.max(10)]],
      description: ['', [Validators.maxLength(500)]],
      eventDate: [this.todayString(), Validators.required]
    });
  }

  ngOnInit(): void {
    console.log('[EventCreate] ngOnInit called');
    this.loadEmotions();
  }

  private todayString(): string {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }

  private loadEmotions(): void {
    console.log('[EventCreate] loadEmotions called');
    this.loadingEmotions = true;
    this.emotionService.getAll().subscribe({
      next: (emotions) => {
        console.log('[EventCreate] emotions loaded:', emotions.length);
        this.emotions = emotions;
        this.loadingEmotions = false;
      },
      error: (err) => {
        console.error('[EventCreate] error:', err);
        this.errorMessage = 'Erro ao carregar emoções. Tente novamente.';
        this.loadingEmotions = false;
      }
    });
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = '';

    if (this.eventForm.invalid) {
      return;
    }

    const user = this.authService.getCurrentUser();
    if (!user) {
      this.errorMessage = 'Usuário não autenticado. Faça login novamente.';
      return;
    }

    this.loading = true;
    const formValue = this.eventForm.value;

    this.eventLogService.create({
      userId: user.id,
      emotionId: formValue.emotionId,
      intensity: formValue.intensity,
      description: formValue.description || '',
      eventDate: new Date(formValue.eventDate).toISOString()
    }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/events']);
      },
      error: (error: Error) => {
        this.errorMessage = error.message;
        this.loading = false;
      }
    });
  }

  get emotionId() { return this.eventForm.get('emotionId'); }
  get intensity() { return this.eventForm.get('intensity'); }
  get description() { return this.eventForm.get('description'); }
  get eventDate() { return this.eventForm.get('eventDate'); }
}
