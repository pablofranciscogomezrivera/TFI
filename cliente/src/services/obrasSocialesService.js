import api from './api';

export const obrasSocialesService = {
    obtenerTodas: async () => {
        const response = await api.get('/obrassociales');
        return response.data;
    }
};

export default obrasSocialesService;