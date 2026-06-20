import { Routes } from '@angular/router';
import { authGuard } from '../app/guards/auth.guard';

export const routes: Routes = [
  // Redirección por defecto
  { path: '', redirectTo: 'cartelera', pathMatch: 'full' },

  {
    path: 'cartelera',
    loadComponent: () => import('./component/cartelera/cartelera.component').then(m => m.CarteleraComponent)
  },
  {
    path: 'evento/crear',
    loadComponent: () => import('./component/crear-evento/crear-evento.component').then(m => m.CrearEventoComponent),
    canActivate: [authGuard]
  },
  {
  path: 'evento/editar/:id',
  loadComponent: () => import('./component/crear-evento/crear-evento.component').then(m => m.CrearEventoComponent),
  canActivate: [authGuard]
  },
  {
    path: 'evento/:id',
    loadComponent: () => import('./component/detalle-evento/detalle-evento.component').then(m => m.DetalleEventoComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./component/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () => import('./component/registro/registro.component').then(m => m.RegistroComponent)
  },

  // Rutas Privadas (Requieren autenticación)
  {
    path: 'mis-eventos',
    loadComponent: () => import('./component/mis-eventos/mis-eventos.component').then(m => m.MisEventosComponent),
    canActivate: [authGuard]
  },
  {
    path: 'mis-inscripciones',
    loadComponent: () => import('./component/mis-inscripciones/mis-inscripciones.component').then(m => m.MisInscripcionesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'evento/:id/asistentes',
    loadComponent: () => import('./component/asistentes/asistentes.component').then(m => m.AsistentesComponent),
    canActivate: [authGuard]
  },
  



  // Ruta 404
  { path: '**', redirectTo: 'cartelera' }
];