import axios from 'axios';

const API_BASE_URL = 'https://localhost:5284/api';

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export default api;