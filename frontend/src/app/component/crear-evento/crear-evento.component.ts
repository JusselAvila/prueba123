import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventoService } from '../../servicios/evento.servicio';
import { EventoCrear } from '../../modelos/evento.modelo';

@Component({
  selector: 'app-crear-evento',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './crear-evento.component.html',
  styleUrl: './crear-evento.component.css'
})
export class CrearEventoComponent implements OnInit {
  private fb = inject(FormBuilder);
  private eventoService = inject(EventoService);
  private router = inject(Router);
  private route = inject(ActivatedRoute); 

  fechaMinima: string = new Date().toISOString().split('T')[0];
  isEditMode = false;
  eventoId: number | null = null;
  isLoading = signal(false);
  errorMessage = signal<string | null>(null);
  

  // TODO: lista de categorías es una suposición (texto libre con sugerencias).
  // Confirma con el backend si existe un catálogo/enum fijo de categorías.
  categoriasSugeridas = ['Música', 'Tecnología', 'Deportes', 'Arte', 'Educación', 'Networking', 'Gastronomía'];

  eventoForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.maxLength(150)]],
    descripcion: ['', [Validators.required]],
    fecha: ['', [Validators.required]],
    horario: ['', [Validators.required]],
    ubicacion: ['', [Validators.required]],
    direccion: ['', [Validators.required]],
    aforoMaximo: [10, [Validators.required, Validators.min(1)]],
    esEventoGrupal: [false],
    esGratuito: [true],
    precio: [0, [Validators.min(0)]],
    categoria: ['', [Validators.required]],
    urlImagen: ['']
  });

  get nombre() { return this.eventoForm.get('nombre'); }
  get descripcion() { return this.eventoForm.get('descripcion'); }
  get fecha() { return this.eventoForm.get('fecha'); }
  get horario() { return this.eventoForm.get('horario'); }
  get ubicacion() { return this.eventoForm.get('ubicacion'); }
  get direccion() { return this.eventoForm.get('direccion'); }
  get aforoMaximo() { return this.eventoForm.get('aforoMaximo'); }
  get esGratuito() { return this.eventoForm.get('esGratuito'); }
  get precio() { return this.eventoForm.get('precio'); }
  get categoria() { return this.eventoForm.get('categoria'); }

  ngOnInit(): void {
    // 1. La lógica de suscripción (la que ya tenías)
    this.esGratuito?.valueChanges.subscribe((esGratuito) => {
      if (esGratuito) {
        this.precio?.setValue(0);
        this.precio?.clearValidators();
        this.precio?.addValidators(Validators.min(0));
      } else {
        this.precio?.clearValidators();
        this.precio?.addValidators([Validators.required, Validators.min(0.01)]);
      }
      this.precio?.updateValueAndValidity();
    });

    // 2. La lógica de detectar modo edición (la nueva)
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.eventoId = Number(id);
      this.cargarEventoParaEdicion(this.eventoId);
    }
  }

  cargarEventoParaEdicion(id: number): void {
  this.eventoService.obtenerDetalle(id).subscribe({
    next: (evento) => {
      // Si la fecha llega como objeto Date o string, la normalizamos
      const fechaFormateada = new Date(evento.fecha).toISOString().split('T')[0];
      
      // Rellenamos el formulario
      this.eventoForm.patchValue({
        ...evento,
        fecha: fechaFormateada
      });
    },
    error: () => this.errorMessage.set('Error al cargar datos para editar.')
  });
  }

  onSubmit(): void {
    if (this.eventoForm.invalid) {
      this.eventoForm.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.isLoading.set(true);

    const evento: EventoCrear = this.eventoForm.getRawValue() as EventoCrear;

    const operacion = (this.isEditMode && this.eventoId)
      ? this.eventoService.actualizarEvento(this.eventoId, evento)
      : this.eventoService.crearEvento(evento);

    operacion.subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/mis-eventos']);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set(this.isEditMode 
          ? 'No se pudo actualizar el evento.' 
          : 'No se pudo crear el evento.');
      }
    });
  }
}