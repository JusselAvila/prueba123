import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from './servicios/auth.servicio';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(private router: Router, private authService: AuthService) {}

  estaLogueado(): boolean {
    return this.authService.estaAutenticado();
  }

  nombreUsuarioActual(): string | null {
    return this.authService.getUsuarioActual()?.nombre ?? null;
  }

  cerrarSesion() {
    this.authService.logout();
    this.router.navigate(['/cartelera']);
  }
}