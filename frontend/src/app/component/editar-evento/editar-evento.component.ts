import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventoService } from '../../servicios/evento.servicio';

@Component({
  selector: 'app-editar-evento',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './editar-evento.component.html'
})
export class EditarEventoComponent implements OnInit {
  
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private eventoService = inject(EventoService);


  loginForm = this.fb.group({
    nombre: ['', Validators.required],
  });

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.eventoService.obtenerDetalle(id).subscribe(data => {
       this.loginForm.patchValue(data);
    });
  }

  onSubmit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    const datos = this.loginForm.getRawValue();
    this.eventoService.actualizarEvento(id, datos as any).subscribe();
  }
}