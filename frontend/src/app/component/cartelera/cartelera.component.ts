import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EventoService } from '../../servicios/evento.servicio';
import { AuthService } from '../../servicios/auth.servicio';
import { EventoLista } from '../../modelos/evento.modelo';

@Component({
  selector: 'app-cartelera',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cartelera.component.html',
  styleUrl: './cartelera.component.css'
})
export class CarteleraComponent implements OnInit {
  private eventoService = inject(EventoService);
  authService = inject(AuthService); // público: el template lo usa para mostrar/ocultar "Crear evento"

  eventos = signal<EventoLista[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  // TODO: filtros 100% client-side por ahora. Si tu backend soporta
  // query params (?categoria=X&gratis=true) en /api/evento/cartelera,
  // sería más eficiente mandarlos al servidor en vez de filtrar aquí
  // sobre todo el arreglo descargado.
  categoriaSeleccionada = signal<string>('todas');
  soloGratuitos = signal(false);

  categorias = computed(() => {
    const unicas = new Set(this.eventos().map(e => e.categoria));
    return ['todas', ...Array.from(unicas)];
  });

  eventosFiltrados = computed(() => {
    return this.eventos().filter(evento => {
      const coincideCategoria =
        this.categoriaSeleccionada() === 'todas' || evento.categoria === this.categoriaSeleccionada();
      const coincideGratuito = !this.soloGratuitos() || evento.esGratuito;
      return coincideCategoria && coincideGratuito;
    });
  });

  ngOnInit(): void {
    this.cargarEventos();
  }

  private cargarEventos(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.eventoService.obtenerCartelera().subscribe({
      next: (eventos) => {
        this.eventos.set(eventos);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudo cargar la cartelera de eventos.');
        this.isLoading.set(false);
      }
    });
  }

  onCategoriaChange(event: Event): void {
    const valor = (event.target as HTMLSelectElement).value;
    this.categoriaSeleccionada.set(valor);
  }

  onSoloGratuitosChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.soloGratuitos.set(checked);
  }
}