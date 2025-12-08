import api from './api';

export const pacientesService = {
    registrarPaciente: async (data) => {
        const response = await api.post('/pacientes', data);
        return response.data;
    },

    buscarPaciente: async (cuil) => {
        try {
            const response = await api.get(`/pacientes/${cuil}`);
            return response.data;
        } catch (error) {
            if (error.response?.status === 404) {
                return null;
            }
            throw error;
        }
    },
};

export default pacientesService;