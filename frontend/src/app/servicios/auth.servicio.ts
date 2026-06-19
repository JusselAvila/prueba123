import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Login, LoginRespuesta, Registro } from '../modelos/auth.modelo';


export interface UsuarioActual {
  usuarioId: number;
  nombre: string;
  rol: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  // Asegúrate de que esta URL coincida con tu backend
  private apiUrl = 'http://localhost:5158/api/auth';
  private tokenKey = 'jwt_token';
  private usuarioKey = 'usuario_actual';

  login(credenciales: Login) {
    return this.http.post<LoginRespuesta>(`${this.apiUrl}/login`, credenciales).pipe(
      tap(respuesta => {
        localStorage.setItem(this.tokenKey, respuesta.token);
        // Guardamos los datos del usuario para no depender de decodificar el JWT
        // ni de pedirlos de nuevo al backend en cada componente.
        const usuario: UsuarioActual = {
          usuarioId: respuesta.usuarioId,
          nombre: respuesta.nombre,
          rol: respuesta.rol
        };
        localStorage.setItem(this.usuarioKey, JSON.stringify(usuario));
      })
    );
  }

  registro(datos: Registro) {
    return this.http.post(`${this.apiUrl}/registro`, datos);
  }

  logout() {

    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.usuarioKey);

  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  estaAutenticado(): boolean {
    return !!this.getToken();
  }

  getUsuarioActual(): UsuarioActual | null {
    const data = localStorage.getItem(this.usuarioKey);
    return data ? JSON.parse(data) : null;
  }

}