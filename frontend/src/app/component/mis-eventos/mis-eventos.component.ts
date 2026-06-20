import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventoService } from '../../servicios/evento.servicio';
import { EventoLista } from '../../modelos/evento.modelo';


@Component({
  selector: 'app-mis-eventos',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './mis-eventos.component.html',
  styleUrl: './mis-eventos.component.css'
})
export class MisEventosComponent implements OnInit {
  private eventoService = inject(EventoService);

  eventos = signal<EventoLista[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.cargarMisEventos();
  }

  private cargarMisEventos(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.eventoService.obtenerMisEventos().subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set(
          'No se pudieron cargar tus eventos. Verifica que el endpoint /api/evento/mis-eventos exista en el backend.'
        );
        this.isLoading.set(false);
      }
    });
  }

  eliminarEvento(id: number): void {
    this.eventoService.eliminarEvento(id).subscribe({
      next: () => {
        this.eventos.update(list => list.filter(e => e.eventoId !== id));
      },
      error: () => alert('Error al eliminar el evento.')
    });
  }

}

