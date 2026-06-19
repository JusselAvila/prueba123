import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventoService } from '../../servicios/evento.servicio';
import { InscripcionService } from '../../servicios/inscripcion.servicio';
import { AuthService } from '../../servicios/auth.servicio';
import { EventoDetalle } from '../../modelos/evento.modelo';


@Component({
  selector: 'app-detalle-evento',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './detalle-evento.component.html',
  styleUrl: './detalle-evento.component.css'
})
export class DetalleEventoComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private eventoService = inject(EventoService);
  private inscripcionService = inject(InscripcionService);
  private fb = inject(FormBuilder);
  authService = inject(AuthService);

  evento = signal<EventoDetalle | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  yaInscrito = signal(false);
  isInscribiendo = signal(false);
  inscripcionExitosa = signal(false);
  inscripcionError = signal<string | null>(null);

  pagoForm = this.fb.group({
    referenciaPago: ['']
  });

  ngOnInit(): void {

    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');


      if (!idParam) {
        console.warn("Esperando parámetros de la ruta...");
        return; 
      }

      const id = Number(idParam);


      if (isNaN(id)) {
        console.error("El ID del evento no es un número válido");
        return;
      }

  
      this.cargarEvento(id);

      if (this.authService.estaAutenticado()) {
        this.verificarInscripcionPrevia(id);
      }
    });
  }

  private cargarEvento(id: number): void {
    this.isLoading.set(true);
    this.eventoService.obtenerDetalle(id).subscribe({
      next: (evento) => {
        this.evento.set(evento);
        this.isLoading.set(false);

        // Si el evento no es gratuito, la referencia de pago pasa a ser obligatoria
        if (!evento.esGratuito) {
          this.pagoForm.get('referenciaPago')?.addValidators(Validators.required);
          this.pagoForm.get('referenciaPago')?.updateValueAndValidity();
        }
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar la información del evento.');
        this.isLoading.set(false);
      }
    });
  }

  verificarInscripcionPrevia(eventoId: number): void {
    this.inscripcionService.verificar(eventoId).subscribe({
      next: (yaEstaInscrito: boolean) => {
        if (yaEstaInscrito) {
          console.log("El usuario ya está inscrito");
          // Aquí podrías cambiar una bandera para ocultar el botón de inscribirse
        }
      },
      error: (err) => console.error("Error al verificar", err)
    });
  }

  inscribirse(): void {
    const evento = this.evento();
    if (!evento) return;

    if (this.pagoForm.invalid) {
      this.pagoForm.markAllAsTouched();
      return;
    }

    this.isInscribiendo.set(true);
    this.inscripcionError.set(null);

    const referenciaPago = this.pagoForm.value.referenciaPago || undefined;

    this.inscripcionService
      .inscribirse({ eventoId: evento.eventoId, referenciaPago })
      .subscribe({
        next: () => {
          this.isInscribiendo.set(false);
          this.inscripcionExitosa.set(true);
          this.yaInscrito.set(true);
        },
        error: (err) => {
          this.isInscribiendo.set(false);
          // TODO: confirmar con el backend el status code real para "cupo lleno" / "ya inscrito"
          this.inscripcionError.set(
            err.status === 409
              ? 'Ya estás inscrito en este evento o el cupo está lleno.'
              : 'No se pudo completar la inscripción. Intenta de nuevo.'
          );
        }
      });
  }
}