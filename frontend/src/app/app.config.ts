import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

// 1. IMPORTA AQUÍ EL INTERCEPTOR
import { authInterceptor } from './interceptores/auth.interceptores'; 

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    
    // 2. REGÍSTRALO AQUÍ
    provideHttpClient(withInterceptors([authInterceptor])) 
  ]
};