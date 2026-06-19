import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { InscripcionService } from '../../servicios/inscripcion.servicio';
import { Asistencia } from '../../modelos/inscripcion.modelo';

@Component({
  selector: 'app-asistentes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './asistentes.component.html',
  styleUrl: './asistentes.component.css'
})
export class AsistentesComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private inscripcionService = inject(InscripcionService);

  eventoId = 0;
  asistentes = signal<Asistencia[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  guardandoId = signal<number | null>(null);
  errorGuardado = signal<string | null>(null);

  ngOnInit(): void {
    this.eventoId = Number(this.route.snapshot.paramMap.get('id'));
    this.cargarAsistentes();
  }

  private cargarAsistentes(): void {
    this.isLoading.set(true);
    this.inscripcionService.obtenerAsistentes(this.eventoId).subscribe({
      next: (asistentes) => {
        this.asistentes.set(asistentes);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar la lista de asistentes.');
        this.isLoading.set(false);
      }
    });
  }

  // NOTA: depende de InscripcionService.marcarAsistencia(), que llama a
  // PATCH /api/inscripcion/{id}/asistencia. Ese endpoint NO EXISTE todavía
  // en tu backend — ver el TODO en inscripcion.servicio.ts. Hasta que lo
  // agregues, este checkbox va a fallar silenciosamente (revertirá el valor).
  toggleAsistencia(asistente: Asistencia): void {
    const nuevoValor = !asistente.asistio;
    this.guardandoId.set(asistente.inscripcionId);
    this.errorGuardado.set(null);

  this.inscripcionService.marcarAsistencia(asistente.inscripcionId).subscribe({
    next: () => {
      // Actualizamos localmente el estado inverso al que tenía
      this.asistentes.update(lista =>
        lista.map(a => 
          (a.inscripcionId === asistente.inscripcionId ? { ...a, asistio: !a.asistio } : a)
        )
      );
      this.guardandoId.set(null);
    },
    error: () => {
      this.guardandoId.set(null);
      this.errorGuardado.set('No se pudo guardar el cambio.');
    }
  });



  }
}