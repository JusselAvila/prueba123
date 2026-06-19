import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InscripcionCrear, Asistencia, InscripcionDetalle } from '../modelos/inscripcion.modelo';
import { ConfirmarPago } from '../modelos/pago.modelo'; 

@Injectable({ providedIn: 'root' })
export class InscripcionService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5158/api/inscripcion';

  inscribirse(data: InscripcionCrear): Observable<any> {
    return this.http.post(`${this.apiUrl}/inscribirse`, data);
  }
  misInscripciones(): Observable<InscripcionDetalle[]> {
    return this.http.get<InscripcionDetalle[]>(`${this.apiUrl}/mis-inscripciones`);
  }

  obtenerAsistentes(eventoId: number): Observable<Asistencia[]> {
    return this.http.get<Asistencia[]>(`${this.apiUrl}/asistentes/${eventoId}`);
  }

  marcarAsistencia(inscripcionId: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/asistencia/${inscripcionId}`, {});
  }

  // Método corregido (debe ser minúscula confirmarPago)
  confirmarPago(pagoData: ConfirmarPago): Observable<any> {
    return this.http.post(`${this.apiUrl}/confirmar-pago`, pagoData);
  }

  verificar(eventoId: number): Observable<boolean> {
  return this.http.get<boolean>(`${this.apiUrl}/esta-inscrito/${eventoId}`);
  }
}