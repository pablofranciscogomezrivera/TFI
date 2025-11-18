import api from './api';

export const pacientesService = {
    // Registrar nuevo paciente
    registrarPaciente: async (data) => {
        const response = await api.post('/pacientes', data);
        return response.data;
    },

    // Buscar paciente por CUIL
    buscarPaciente: async (cuil) => {
        try {
            // Nota: Este endpoint no está documentado, pero se necesitaría para verificar si existe
            // Por ahora intentaremos registrar directamente
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