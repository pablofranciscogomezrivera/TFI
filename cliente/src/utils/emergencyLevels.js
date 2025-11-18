import { NivelEmergencia, getNivelEmergenciaInfo } from '../constants/enums';

// Mapeo de niveles de emergencia a variantes de badge
export const getBadgeVariantByEmergency = (nivel) => {
    const variants = {
        [NivelEmergencia.CRITICA]: 'critica',
        [NivelEmergencia.EMERGENCIA]: 'emergencia',
        [NivelEmergencia.URGENCIA]: 'urgencia',
        [NivelEmergencia.URGENCIA_MENOR]: 'menor',
        [NivelEmergencia.SIN_URGENCIA]: 'sin-urgencia'
    };
    return variants[nivel] || 'default';
};

// Obtener etiqueta de texto para nivel de emergencia
export const getEmergencyLabel = (nivel) => {
    return getNivelEmergenciaInfo(nivel).label;
};