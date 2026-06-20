export interface Asistencia {
    inscripcionId: number;
    nombreUsuario: string;
    correoUsuario: string;
    asistio: boolean;
    pagoConfirmado: boolean;
}

export interface InscripcionDetalle {
    inscripcionId: number;
    nombreEvento: string;
    fechaInscripcion: Date;
    pagoConfirmado: boolean;
    estado: string;
}

export interface InscripcionCrear {
    eventoId: number;
    referenciaPago?: string;
}