import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InscripcionService } from '../../servicios/inscripcion.servicio';
import { InscripcionDetalle } from '../../modelos/inscripcion.modelo';
import { ConfirmarPago } from '../../modelos/pago.modelo'; 


@Component({
  selector: 'app-mis-inscripciones',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mis-inscripciones.component.html',
  styleUrl: './mis-inscripciones.component.css'
})
export class MisInscripcionesComponent implements OnInit {
  private inscripcionService = inject(InscripcionService);

  inscripciones = signal<InscripcionDetalle[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.cargarInscripciones();
  }

  private cargarInscripciones(): void {
    this.inscripcionService.misInscripciones().subscribe({
      next: (inscripciones) => {
        this.inscripciones.set(inscripciones);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('No se pudieron cargar tus inscripciones.');
        this.isLoading.set(false);
      }
    });
  }


  confirmarPago(id: number) {
      console.log("=== DEPURACIÓN DE PAGO ===");
      console.log("ID que estoy enviando al servidor:", id);
      
      // Ahora enviamos ambos campos como espera el DTO del backend
      const data: ConfirmarPago = { 
        inscripcionId: id, 
        referencia: "SIMULACION" 
      };

      this.inscripcionService.confirmarPago(data).subscribe({
        next: (res) => {
          alert("Pago confirmado");
          this.cargarInscripciones(); 
        },
        error: (err) => {
          console.error("Respuesta del servidor:", err);
          // Intentamos obtener el mensaje del error de forma más segura
          const mensajeError = err.error?.mensaje || err.error || "Error desconocido";
          alert('Error: ' + mensajeError);
        }
      });
    }

  verificarId(id: number) {
    alert('El ID detectado es: ' + id);
  }

}