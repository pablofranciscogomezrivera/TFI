import api from './api';

export const urgenciasService = {
    // Obtener lista de espera (pendientes)
    obtenerListaEspera: async () => {
        const response = await api.get('/urgencias/lista-espera');
        return response.data;
    },

    // Obtener ingresos en proceso (reclamados pero no finalizados)
    obtenerIngresosEnProceso: async () => {
        const response = await api.get('/urgencias/en-proceso');
        return response.data;
    },

    // Registrar nueva urgencia
    // NOTA: Ya NO se envía la matrícula como header
    // El backend la extrae del JWT automáticamente por seguridad
    registrarUrgencia: async (data) => {
        const response = await api.post('/urgencias', data);
        return response.data;
    },

    // Reclamar paciente (para médicos)
    // NOTA: Ya NO se envía la matrícula como header
    // El backend la extrae del JWT automáticamente por seguridad
    reclamarPaciente: async () => {
        const response = await api.post('/urgencias/reclamar');
        return response.data;
    },

    // Registrar atención médica
    // NOTA: Ya NO se envía la matrícula como header
    // El backend la extrae del JWT automáticamente por seguridad
    registrarAtencion: async (cuilPaciente, informeMedico) => {
        const response = await api.post(`/atenciones/${cuilPaciente}`, {
            informeMedico: informeMedico
        });
        return response.data;
    }
};

export default urgenciasService;
