import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit, OnDestroy {
  loginForm: FormGroup;
  submitted: boolean = false;
  loading: boolean = false;
  errorMessage: string = '';

  rotatingPhrases: string[] = [
    'debugue suas emoções.',
    'compile seus sentimentos.',
    'resolva seus bugs internos.',
    'refatore seu bem-estar.',
    'deploy sua melhor versão.'
  ];
  currentPhrase = '';
  private phraseIndex = 0;
  private phraseInterval: ReturnType<typeof setInterval> | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.currentPhrase = this.rotatingPhrases[0];
    this.phraseInterval = setInterval(() => {
      this.phraseIndex = (this.phraseIndex + 1) % this.rotatingPhrases.length;
      this.currentPhrase = this.rotatingPhrases[this.phraseIndex];
    }, 4000);
  }

  ngOnDestroy(): void {
    if (this.phraseInterval) {
      clearInterval(this.phraseInterval);
    }
  }

  onSubmit(): void {
    this.submitted = true;
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/home']);
      },
      error: (error: Error) => {
        this.errorMessage = error.message;
        this.loading = false;
      }
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }
}
