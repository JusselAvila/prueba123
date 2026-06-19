import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventoLista, EventoDetalle, EventoCrear } from '../modelos/evento.modelo';

@Injectable({ providedIn: 'root' })
export class EventoService {
  private http = inject(HttpClient);
  // Ajusta esta URL a la de tu backend real
  private apiUrl = 'http://localhost:5158/api/evento';

  obtenerCartelera(): Observable<EventoLista[]> {
    return this.http.get<EventoLista[]>(`${this.apiUrl}/cartelera`);
  }

  obtenerDetalle(id: number): Observable<EventoDetalle> {
    return this.http.get<EventoDetalle>(`${this.apiUrl}/${id}`);
  }

  crearEvento(evento: EventoCrear): Observable<any> {
    return this.http.post(`${this.apiUrl}/crear`, evento);
  }

  obtenerMisEventos(): Observable<EventoLista[]> {
    return this.http.get<EventoLista[]>(`${this.apiUrl}/mis-eventos`);
  }

}