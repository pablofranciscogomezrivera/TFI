import axios from 'axios';

const API_BASE_URL = 'https://localhost:5284/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('authToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
            console.log('Token agregado a headers:', token.substring(0, 20) + '...');
        } else {
            console.warn('No hay token en localStorage!');
        }

        // La matrícula se extrae del token JWT en el backend por seguridad
        // Esto previene la suplantación de identidad

        console.log('Request URL:', config.url);
        console.log('Request Headers:', config.headers);

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Interceptor de respuesta para manejar errores de manera elegante
api.interceptors.response.use(
    (response) => {
        // Si la respuesta es exitosa, simplemente la retornamos
        return response;
    },
    (error) => {
        // Manejo específico de errores de autenticación (401)
        if (error.response && error.response.status === 401) {
            console.warn('⚠️ Sesión expirada o no autorizado');

            // Crear un mensaje amigable para el usuario
            const errorMessage = error.response.data?.message || 'Tu sesión ha expirado';

            // Mostrar notificación al usuario (si existe un sistema de notificaciones global)
            // Puedes implementar un event emitter o usar un context de React para esto
            if (window.showNotification) {
                window.showNotification(errorMessage, 'error');
            }

            // Limpiar datos de autenticación
            localStorage.removeItem('authToken');
            localStorage.removeItem('matricula');
            localStorage.removeItem('userRole');

            // Dar tiempo al usuario para ver el mensaje antes de redirigir
            setTimeout(() => {
                // Redirigir al login con un mensaje de por qué fue redirigido
                window.location.href = '/login?reason=session_expired';
            }, 2000); // 2 segundos para que el usuario vea el mensaje
        }

        // Manejo de otros errores comunes
        if (error.response) {
            // El servidor respondió con un código de error
            console.error('Error de servidor:', {
                status: error.response.status,
                data: error.response.data,
                url: error.config?.url
            });
        } else if (error.request) {
            // La petición fue hecha pero no hubo respuesta
            console.error('Sin respuesta del servidor:', error.request);
        } else {
            // Algo pasó al configurar la petición
            console.error('Error al configurar petición:', error.message);
        }

        return Promise.reject(error);
    }
);

export default api;