import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../servicios/auth.servicio';
import { Login } from '../../modelos/auth.modelo';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  // Estado de UI con signals (idiomático en Angular 17+)
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  loginForm = this.fb.group({
    correo: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  // Getters para acceder fácil a los controles desde el HTML
  get correo() {
    return this.loginForm.get('correo');
  }

  get password() {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.isLoading.set(true);

    const credenciales = this.loginForm.getRawValue() as Login;

    this.authService.login(credenciales).subscribe({
      next: () => {
        this.isLoading.set(false);
        // Ajusta la ruta de redirección a la que use tu app
        this.router.navigate(['/cartelera']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(
          err.status === 401
            ? 'Correo o contraseña incorrectos.'
            : 'Ocurrió un error al iniciar sesión. Intenta de nuevo.'
        );
      }
    });
  }
}