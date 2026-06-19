import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './servicios/auth.servicio'; // 1. IMPORTA EL SERVICIO

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule],
  templateUrl: './app.html' 
})
export class App {
  // 2. INYECTA EL SERVICIO EN EL CONSTRUCTOR
  constructor(private router: Router, private authService: AuthService) {}
  
  estaLogueado(): boolean {
    // 3. USA EL MÉTODO DEL SERVICIO QUE YA TIENES BIEN HECHO
    return this.authService.estaAutenticado();
  }

  cerrarSesion() {
    // 4. USA EL MÉTODO DEL SERVICIO PARA EL LOGOUT
    this.authService.logout();
    this.router.navigate(['/cartelera']);
  }
}