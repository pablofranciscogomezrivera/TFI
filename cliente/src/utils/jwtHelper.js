/**
 * Helper para trabajar con JWT
 */

/**
 * Decodifica un JWT sin verificar la firma (solo para debug en frontend)
 * IMPORTANTE: Esto NO verifica la autenticidad del token
 */
export const decodeJWT = (token) => {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch (error) {
        console.error('Error decodificando JWT:', error);
        return null;
    }
};

/**
 * Obtiene un claim específico del JWT almacenado
 */
export const getClaimFromStoredToken = (claimName) => {
    const token = localStorage.getItem('authToken');
    if (!token) {
        console.warn('No hay token en localStorage');
        return null;
    }

    const payload = decodeJWT(token);
    return payload?.[claimName];
};

/**
 * Verifica si el token tiene un claim específico
 */
export const hasClaimInToken = (claimName) => {
    const claim = getClaimFromStoredToken(claimName);
    return claim !== null && claim !== undefined;
};

/**
 * Debug: Muestra todos los claims del token actual en la consola
 */
export const debugToken = () => {
    const token = localStorage.getItem('authToken');
    if (!token) {
        console.warn('❌ No hay token en localStorage');
        return;
    }

    const payload = decodeJWT(token);
    console.log('🔍 JWT Payload:', payload);
    console.log('📝 Claims disponibles:', Object.keys(payload || {}));

    // Verificar claims importantes
    const importantClaims = ['nameid', 'email', 'role', 'Matricula'];
    importantClaims.forEach(claim => {
        const value = payload?.[claim];
        if (value) {
            console.log(`✅ ${claim}: ${value}`);
        } else {
            console.warn(`⚠️ ${claim}: NO ENCONTRADO`);
        }
    });

    return payload;
};
