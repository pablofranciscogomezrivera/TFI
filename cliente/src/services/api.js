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

        const matricula = localStorage.getItem('matricula');
        if (matricula) {
            config.headers['X-Enfermera-Matricula'] = matricula;
            config.headers['X-Doctor-Matricula'] = matricula;
            console.log('Matrícula agregada:', matricula);
        }
        console.log('Request URL:', config.url);
        console.log('Request Headers:', config.headers);

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default api;