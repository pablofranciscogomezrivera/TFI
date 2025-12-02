/**
 * Enumeraciones del backend - Sincronizadas con Dominio/Entidades
 */

export const NivelEmergencia = {
    CRITICA: 0,
    EMERGENCIA: 1,
    URGENCIA: 2,
    URGENCIA_MENOR: 3,
    SIN_URGENCIA: 4
};

export const EstadoIngreso = {
    PENDIENTE: 0,
    EN_PROCESO: 1,
    FINALIZADO: 2
};

export const NivelesEmergencia = {
    [NivelEmergencia.CRITICA]: {
        id: NivelEmergencia.CRITICA,
        nombre: 'Crítica',
        tiempoEspera: 'Inmediato',
        color: 'Rojo',      
        hex: '#ef4444'       
    },
    [NivelEmergencia.EMERGENCIA]: {
        id: NivelEmergencia.EMERGENCIA,
        nombre: 'Emergencia',
        tiempoEspera: '10-30 min',
        color: 'Naranja',
        hex: '#f97316'
    },
    [NivelEmergencia.URGENCIA]: {
        id: NivelEmergencia.URGENCIA,
        nombre: 'Urgencia',
        tiempoEspera: '60 min',
        color: 'Amarillo',
        hex: '#f59e0b'
    },
    [NivelEmergencia.URGENCIA_MENOR]: {
        id: NivelEmergencia.URGENCIA_MENOR,
        nombre: 'Urgencia Menor',
        tiempoEspera: '2 horas',
        color: 'Verde',
        hex: '#10b981'
    },
    [NivelEmergencia.SIN_URGENCIA]: {
        id: NivelEmergencia.SIN_URGENCIA,
        nombre: 'Sin Urgencia',
        tiempoEspera: '4 horas',
        color: 'Azul',
        hex: '#3b82f6'
    }
};

export const getNivelEmergenciaInfo = (nivel) => {
    const n = NivelesEmergencia[nivel];
    if (!n) return NivelesEmergencia[NivelEmergencia.URGENCIA];

    return {
        label: n.nombre,
        color: n.hex,
        tiempo: n.tiempoEspera,
        descripcion: `Prioridad: ${n.nombre}`
    };
};

export const getEstadoIngresoLabel = (estado) => {
    const labels = {
        [EstadoIngreso.PENDIENTE]: 'Pendiente',
        [EstadoIngreso.EN_PROCESO]: 'En Proceso',
        [EstadoIngreso.FINALIZADO]: 'Finalizado'
    };
    return labels[estado] || 'Desconocido';
};

export default {
    NivelEmergencia,
    NivelesEmergencia, 
    EstadoIngreso,
    getNivelEmergenciaInfo,
    getEstadoIngresoLabel
};