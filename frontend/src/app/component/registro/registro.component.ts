import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import { AuthService } from '../../servicios/auth.servicio';
import { Registro } from '../../modelos/auth.modelo';

// Validador a nivel de FormGroup: compara password y confirmPassword
function passwordsIgualesValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordsNoCoinciden: true };
}

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './registro.component.html',
  styleUrl: './registro.component.css'
})
export class RegistroComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoading = signal(false);
  errorMessage = signal<string | null>(null);

  registroForm = this.fb.group(
    {
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      correo: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    },
    { validators: passwordsIgualesValidator }
  );

  get nombre() {
    return this.registroForm.get('nombre');
  }

  get correo() {
    return this.registroForm.get('correo');
  }

  get password() {
    return this.registroForm.get('password');
  }

  get confirmPassword() {
    return this.registroForm.get('confirmPassword');
  }

  onSubmit(): void {
    if (this.registroForm.invalid) {
      this.registroForm.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.isLoading.set(true);

    // confirmPassword es solo validación de UI, no se envía al backend
    const { confirmPassword, ...datos } = this.registroForm.getRawValue();
    const registroData: Registro = datos as Registro;

    this.authService.registro(registroData).subscribe({
      next: () => {
        this.isLoading.set(false);
        // No hay token todavía: el usuario debe iniciar sesión manualmente
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading.set(false);
        // TODO: confirmar con el backend el status code real para "correo ya registrado"
        this.errorMessage.set(
          err.status === 409
            ? 'Ese correo ya está registrado.'
            : 'Ocurrió un error al registrarte. Intenta de nuevo.'
        );
      }
    });
  }
}