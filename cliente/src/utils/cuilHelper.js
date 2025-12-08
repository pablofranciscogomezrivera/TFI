/**
 * Helper para trabajar con CUILs (Código Único de Identificación Laboral)
 */

/**
 * Normaliza un CUIL removiendo guiones y espacios
 * @param {string} cuil - CUIL con o sin formato
 * @returns {string} CUIL solo con dígitos
 */
export const normalizeCuil = (cuil) => {
    if (!cuil) return '';
    return cuil.replace(/-/g, '').replace(/\s/g, '').trim();
};

/**
 * Formatea un CUIL agregando guiones en formato XX-XXXXXXXX-X
 * @param {string} cuil - CUIL sin formato o con formato
 * @returns {string} CUIL formateado
 */
export const formatCuil = (cuil) => {
    if (!cuil) return '';
    
    const normalized = normalizeCuil(cuil);
    
    // Si ya tiene el formato correcto con guiones, devolverlo
    if (cuil.includes('-') && cuil.length === 13) return cuil;
    
    // Si no tiene 11 dígitos, devolver el original
    if (normalized.length !== 11) return cuil;
    
    // Formatear: XX-XXXXXXXX-X
    return `${normalized.substring(0, 2)}-${normalized.substring(2, 10)}-${normalized.substring(10)}`;
};

/**
 * Valida el formato básico de un CUIL (sin validar dígito verificador)
 * Esta validación es para el frontend. El backend hace la validación completa.
 * @param {string} cuil - CUIL a validar
 * @returns {boolean} true si el formato es válido
 */
export const validateCuilFormat = (cuil) => {
    if (!cuil) return false;
    
    const normalized = normalizeCuil(cuil);
    
    // Debe tener exactamente 11 dígitos
    if (normalized.length !== 11) return false;
    
    // Debe contener solo números
    return /^\d{11}$/.test(normalized);
};

/**
 * Valida un CUIL completo incluyendo el dígito verificador
 * @param {string} cuil - CUIL a validar
 * @returns {boolean} true si el CUIL es válido
 */
export const validateCuil = (cuil) => {
    if (!validateCuilFormat(cuil)) return false;
    
    const normalized = normalizeCuil(cuil);
    const multiplicadores = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];
    
    let suma = 0;
    for (let i = 0; i < 10; i++) {
        suma += parseInt(normalized[i]) * multiplicadores[i];
    }
    
    const resto = suma % 11;
    const digitoVerificador = resto === 0 ? 0 : resto === 1 ? 9 : 11 - resto;
    
    return digitoVerificador === parseInt(normalized[10]);
};
