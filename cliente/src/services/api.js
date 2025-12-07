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

        

        console.log('Request URL:', config.url);
        console.log('Request Headers:', config.headers);

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);


api.interceptors.response.use(
    (response) => {
        
        return response;
    },
    (error) => {
        
        if (error.response && error.response.status === 401) {
            console.warn('⚠️ Sesión expirada o no autorizado');

            
            const errorMessage = error.response.data?.message || 'Tu sesión ha expirado';

            
            if (window.showNotification) {
                window.showNotification(errorMessage, 'error');
            }

            
            localStorage.removeItem('authToken');
            localStorage.removeItem('matricula');
            localStorage.removeItem('userRole');

            
            setTimeout(() => {
                
                window.location.href = '/login?reason=session_expired';
            }, 2000); 
        }

        
        if (error.response) {
            
            console.error('Error de servidor:', {
                status: error.response.status,
                data: error.response.data,
                url: error.config?.url
            });
        } else if (error.request) {
            
            console.error('Sin respuesta del servidor:', error.request);
        } else {
            
            console.error('Error al configurar petición:', error.message);
        }

        return Promise.reject(error);
    }
);

export default api;