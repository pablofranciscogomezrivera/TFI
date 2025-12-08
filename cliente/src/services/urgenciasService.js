import api from './api';

export const urgenciasService = {
    obtenerListaEspera: async () => {
        const response = await api.get('/urgencias/lista-espera');
        return response.data;
    },

    obtenerIngresosEnProceso: async () => {
        const response = await api.get('/urgencias/en-proceso');
        return response.data;
    },

    registrarUrgencia: async (data) => {
        const response = await api.post('/urgencias', data);
        return response.data;
    },

    reclamarPaciente: async () => {
        const response = await api.post('/urgencias/reclamar');
        return response.data;
    },

    cancelarAtencion: async (cuil) => {
        const response = await api.post(`/urgencias/cancelar/${cuil}`);
        return response.data;
    },

    registrarAtencion: async (cuilPaciente, informeMedico) => {
        const response = await api.post('/atenciones', {
            cuilPaciente: cuilPaciente,
            informeMedico: informeMedico
        });
        return response.data;
    }
};

export default urgenciasService;
