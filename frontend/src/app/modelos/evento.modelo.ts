export interface EventoLista {
    eventoId: number;
    nombre: string;
    categoria: string;
    fecha: Date; 
    horario: string;
    aforoMaximo: number;
    inscritosActuales: number;
    esGratuito: boolean;
    precio: number;
    urlImagen?: string;
}

export interface EventoDetalle extends EventoLista {
    descripcion: string;
    ubicacion: string;
    direccion: string;
    esEventoGrupal: boolean;
    nombreOrganizador: string;
}

export interface EventoCrear {
    nombre: string;
    descripcion: string;
    fecha: Date | string;
    horario: string;
    ubicacion: string;
    direccion: string;
    aforoMaximo: number;
    esEventoGrupal: boolean;
    esGratuito: boolean;
    precio: number;
    categoria: string;
    urlImagen?: string;
}