import api from './api';

export const urgenciasService = {
    // Obtener lista de espera
    obtenerListaEspera: async () => {
        const response = await api.get('/urgencias/lista-espera');
        return response.data;
    },

    // Registrar nueva urgencia
    registrarUrgencia: async (data, matriculaEnfermera) => {
        const response = await api.post('/urgencias', data, {
            headers: {
                'X-Enfermera-Matricula': matriculaEnfermera
            }
        });
        return response.data;
    },

    // Reclamar paciente (para médicos)
    reclamarPaciente: async (matriculaDoctor) => {
        const response = await api.post('/urgencias/reclamar', {}, {
            headers: {
                'X-Doctor-Matricula': matriculaDoctor
            }
        });
        return response.data;
    },
};

export default urgenciasService;