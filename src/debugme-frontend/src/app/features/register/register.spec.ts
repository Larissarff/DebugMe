import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Register } from './register';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('Register', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Register],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form when empty', () => {
    expect(component.registerForm.valid).toBeFalsy();
  });

  it('should validate name field', () => {
    const name = component.registerForm.controls['name'];
    name.setValue('A');
    expect(name.valid).toBeFalsy();
    name.setValue('João');
    expect(name.valid).toBeTruthy();
  });

  it('should validate email field', () => {
    const email = component.registerForm.controls['email'];
    email.setValue('invalid');
    expect(email.valid).toBeFalsy();
    email.setValue('test@example.com');
    expect(email.valid).toBeTruthy();
  });

  it('should validate password minimum length', () => {
    const password = component.registerForm.controls['password'];
    password.setValue('12345');
    expect(password.valid).toBeFalsy();
    password.setValue('123456');
    expect(password.valid).toBeTruthy();
  });

  it('should validate password match', () => {
    component.registerForm.controls['password'].setValue('password123');
    component.registerForm.controls['confirmPassword'].setValue('password456');
    expect(component.registerForm.hasError('passwordsMismatch')).toBeTruthy();

    component.registerForm.controls['confirmPassword'].setValue('password123');
    expect(component.registerForm.hasError('passwordsMismatch')).toBeFalsy();
  });

  it('should mark submitted on submit', () => {
    expect(component.submitted).toBeFalsy();
    component.onSubmit();
    expect(component.submitted).toBeTruthy();
  });

  it('should not submit when form invalid', () => {
    component.onSubmit();
    expect(component.loading).toBeFalsy();
  });
});
