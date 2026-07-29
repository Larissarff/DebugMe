import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { EmotionService, CreateEmotionRequest, EmotionWithCount } from '../../../core/services/emotion.service';

@Component({
  selector: 'app-emotion-manage',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './emotion-manage.html',
  styleUrl: './emotion-manage.css'
})
export class EmotionManage implements OnInit {
  emotions: EmotionWithCount[] = [];
  loading = true;
  errorMessage = '';

  // Form
  emotionForm: FormGroup;
  formSubmitted = false;
  formLoading = false;
  formError = '';
  formSuccess = '';

  constructor(
    private emotionService: EmotionService,
    private formBuilder: FormBuilder
  ) {
    this.emotionForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      description: ['', [Validators.maxLength(200)]]
    });
  }

  ngOnInit(): void {
    console.log('[EmotionManage] ngOnInit called');
    this.loadEmotions();
  }

  private loadEmotions(): void {
    console.log('[EmotionManage] loadEmotions called');
    this.loading = true;
    this.errorMessage = '';
    this.emotionService.getAllWithCount().subscribe({
      next: (emotions) => {
        console.log('[EmotionManage] emotions loaded:', emotions.length);
        this.emotions = emotions;
        this.loading = false;
      },
      error: (err) => {
        console.error('[EmotionManage] error:', err);
        this.errorMessage = 'Erro ao carregar emoções. Tente novamente.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    this.formSubmitted = true;
    this.formError = '';
    this.formSuccess = '';

    if (this.emotionForm.invalid) return;

    this.formLoading = true;
    const data: CreateEmotionRequest = {
      name: this.emotionForm.value.name.trim(),
      description: this.emotionForm.value.description?.trim() || undefined
    };

    this.emotionService.create(data).subscribe({
      next: (created) => {
        this.emotions.unshift({ ...created, eventCount: 0 });
        this.formSuccess = `"${created.name}" foi cadastrada com sucesso!`;
        this.emotionForm.reset();
        this.formSubmitted = false;
        this.formLoading = false;
      },
      error: (error: Error) => {
        this.formError = error.message;
        this.formLoading = false;
      }
    });
  }

  get name() { return this.emotionForm.get('name'); }
  get description() { return this.emotionForm.get('description'); }
}
