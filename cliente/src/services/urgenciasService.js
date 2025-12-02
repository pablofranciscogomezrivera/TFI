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
    registrarUrgencia: async (data, matriculaEnfermera) => {
        const response = await api.post('/urgencias', data, {
            headers: {
                'X-Enfermera-Matricula': matriculaEnfermera
            }
        });
        return response.data;
    },

    // Reclamar paciente (para m�dicos)
    reclamarPaciente: async (matriculaDoctor) => {
        const response = await api.post('/urgencias/reclamar', {}, {
            headers: {
                'X-Doctor-Matricula': matriculaDoctor
            }
        });
        return response.data;
    },

    // Registrar atenci�n m�dica
    registrarAtencion: async (cuilPaciente, informeMedico, matriculaDoctor) => {
        const response = await api.post('/atenciones', {
            cuilPaciente,
            informeMedico
        }, {
            headers: {
                'X-Doctor-Matricula': matriculaDoctor
            }
        });
        return response.data;
    },
};

export default urgenciasService;