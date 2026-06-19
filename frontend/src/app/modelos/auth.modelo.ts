export interface Login {
    correo: string;
    password: string;
}

export interface Registro {
    nombre: string;
    correo: string;
    password: string;
}

export interface LoginRespuesta {
    token: string;
    usuarioId: number;
    nombre: string;
    rol: string;
}