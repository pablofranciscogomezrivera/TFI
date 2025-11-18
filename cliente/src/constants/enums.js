/**
 * Enumeraciones del backend - Sincronizadas con Dominio/Entidades
 */

/**
 * Niveles de emergencia según el sistema de triage
 * Sincronizado con: Dominio/Entidades/NivelEmergencia.cs
 */
export const NivelEmergencia = {
    CRITICA: 0,        // Rojo - Inmediato (5 minutos)
    EMERGENCIA: 1,     // Naranja - 10-30 minutos
    URGENCIA: 2,       // Amarillo - 60 minutos
    URGENCIA_MENOR: 3, // Verde - 2 horas
    SIN_URGENCIA: 4    // Azul - 4 horas
};

/**
 * Estados de un ingreso en el sistema
 * Sincronizado con: Dominio/Entidades/EstadoIngreso.cs
 */
export const EstadoIngreso = {
    PENDIENTE: 0,   // En lista de espera
    EN_PROCESO: 1,  // Siendo atendido por un médico
    FINALIZADO: 2   // Atención completada
};

/**
 * Obtiene información detallada del nivel de emergencia
 */
export const getNivelEmergenciaInfo = (nivel) => {
    const info = {
        [NivelEmergencia.CRITICA]: {
            label: 'Critica',
            color: '#ef4444',
            tiempo: 'Inmediato (5 min)',
            descripcion: 'Requiere atencion inmediata'
        },
        [NivelEmergencia.EMERGENCIA]: {
            label: 'Emergencia',
            color: '#f97316',
            tiempo: '10-30 minutos',
            descripcion: 'Requiere atencion urgente'
        },
        [NivelEmergencia.URGENCIA]: {
            label: 'Urgencia',
            color: '#f59e0b',
            tiempo: '60 minutos',
            descripcion: 'Requiere atencion pronta'
        },
        [NivelEmergencia.URGENCIA_MENOR]: {
            label: 'Urgencia Menor',
            color: '#10b981',
            tiempo: '2 horas',
            descripcion: 'Puede esperar hasta 2 horas'
        },
        [NivelEmergencia.SIN_URGENCIA]: {
            label: 'Sin Urgencia',
            color: '#3b82f6',
            tiempo: '4 horas',
            descripcion: 'Puede esperar hasta 4 horas'
        }
    };

    return info[nivel] || info[NivelEmergencia.URGENCIA];
};

/**
 * Obtiene el label del estado de ingreso
 */
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
    EstadoIngreso,
    getNivelEmergenciaInfo,
    getEstadoIngresoLabel
};
