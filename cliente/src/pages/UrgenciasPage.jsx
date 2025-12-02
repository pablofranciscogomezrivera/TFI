import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import DashboardUrgencias from '../components/urgencias/DashboardUrgencias';
import ColaPrioridad from '../components/urgencias/ColaPrioridad';
import FormularioIngreso from '../components/urgencias/FormularioIngreso';
import Button from '../components/ui/Button';
import urgenciasService from '../services/urgenciasService';
import pacientesService from '../services/pacientesService'; 
import Notification from '../components/ui/Notification';
import './UrgenciasPage.css';
import React from 'react';

export const UrgenciasPage = () => {
    const [pacientes, setPacientes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [notification, setNotification] = useState(null);
    const [error, setError] = useState(null);

    // Estados para el flujo de ingreso
    const [mostrarFormulario, setMostrarFormulario] = useState(false);
    const [vistaActual, setVistaActual] = useState('cola');

    const userRole = parseInt(localStorage.getItem('userRole'));
    const esMedico = userRole === 0;
    const esEnfermera = userRole === 1;

    const navigate = useNavigate();

    const dniEnfermera = localStorage.getItem('profesionalDNI') || '';
    const matriculaDoctor = localStorage.getItem('matricula') || '';

    const showNotification = (message, type = 'info') => {
        setNotification({ message, type });
    };

    const cargarPacientes = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await urgenciasService.obtenerListaEspera();
            setPacientes(data);
        } catch (err) {
            console.error('Error al cargar pacientes:', err);
            if (err.response?.status === 401) navigate('/login');
            setError('No se pudo cargar la lista de pacientes');
            showNotification('No se pudo actualizar la lista', 'error');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        cargarPacientes();
        const interval = setInterval(cargarPacientes, 30000);
        return () => clearInterval(interval);
    }, []);

    const handleRegistrarIngresoCompleto = async (urgenciaData, pacienteNuevoData) => {
        try {
            // 1. Si viene pacienteNuevoData, primero registramos al paciente
            if (pacienteNuevoData) {
                try {
                    await pacientesService.registrarPaciente(pacienteNuevoData);
                    showNotification('Paciente nuevo registrado automáticamente.', 'success');
                } catch (err) {
                    // Si falla el registro del paciente, detenemos todo
                    throw new Error(err.response?.data?.message || 'Error al registrar datos del paciente.');
                }
            }

            // 2. Registramos la urgencia (el paciente ya existe en BD ahora)
            await urgenciasService.registrarUrgencia(urgenciaData, dniEnfermera);

            showNotification('Ingreso registrado exitosamente.', 'success');
            await cargarPacientes();

            // Volver a la cola
            setMostrarFormulario(false);
            setVistaActual('cola');

        } catch (err) {
            console.error(err);
            const msg = err.message || err.response?.data?.message || 'Error al procesar el ingreso.';
            showNotification(msg, 'error');
        }
    };

    const iniciarNuevoIngreso = () => {
        setVistaActual('formulario');
        setMostrarFormulario(true);
    };

    const handleReclamarPaciente = async () => {
        try {
            const paciente = await urgenciasService.reclamarPaciente(matriculaDoctor);

            // Aquí podrías usar un Modal o una notificación persistente/larga
            // Usamos success por ahora
            showNotification(`ASIGNADO: ${paciente.nombrePaciente} ${paciente.apellidoPaciente}. Revise el informe.`, 'success');

            await cargarPacientes();
        } catch (err) {
            const msg = err.response?.data?.message || 'No hay pacientes en espera.';
            showNotification(msg, 'info'); // Info porque "no hay pacientes" no es necesariamente un error grave
        }
    };

    const handleLogout = () => {
        localStorage.clear();
        navigate('/login');
    };

    const cancelarFlujo = () => {
        
        setMostrarFormulario(false);
        setVistaActual('cola');
    };

    return (
        <div className="urgencias-page">
            {/* Renderizar Notificación Global */}
            {notification && (
                <Notification
                    message={notification.message}
                    type={notification.type}
                    onClose={() => setNotification(null)}
                />
            )}
            <header className="urgencias-header">
                <div className="header-content" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <div>
                        <h1 className="page-title">Módulo de Urgencias</h1>
                        <p className="page-subtitle">
                            Conectado como: {esMedico ? 'Médico' : 'Enfermera'}
                        </p>
                    </div>
                    <Button variant="outline" onClick={handleLogout}>Cerrar Sesión</Button>
                </div>
            </header>

            <div className="urgencias-content">
                <div className="tabs-navigation">
                    <button
                        className={`tab-button ${vistaActual === 'cola' ? 'tab-button-active' : ''}`}
                        onClick={() => cancelarFlujo()}
                    >
                        Cola de Prioridad
                    </button>

                    {esEnfermera && (
                        <button
                            className={`tab-button ${vistaActual === 'formulario' ? 'tab-button-active' : ''}`}
                            onClick={iniciarNuevoIngreso}
                        >
                            Nuevo Ingreso
                        </button>
                    )}
                </div>

                {vistaActual === 'cola' && (
                    <>
                        <DashboardUrgencias pacientes={pacientes} />

                        {esMedico && (
                            <div style={{ marginBottom: '20px', padding: '20px', background: '#e0f2fe', borderRadius: '8px', border: '1px solid #bae6fd', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                <div>
                                    <h3 style={{ margin: 0, color: '#0369a1' }}>Panel Médico</h3>
                                    <p style={{ margin: 0, color: '#0c4a6e' }}>Hay {pacientes.length} pacientes esperando atención.</p>
                                </div>
                                <Button onClick={handleReclamarPaciente} size="large">
                                    Llamar Siguiente Paciente
                                </Button>
                            </div>
                        )}

                        {loading && !pacientes.length ? (
                            <div className="loading-container"><p>Cargando...</p></div>
                        ) : (
                            <ColaPrioridad
                                pacientes={pacientes}
                                onActualizar={cargarPacientes}
                            />
                        )}
                    </>
                )}

                {vistaActual === 'formulario' && mostrarFormulario && (
                    <div className="formulario-section">
                        <FormularioIngreso
                            onSubmit={handleRegistrarIngresoCompleto} // Pasamos el nuevo handler
                            onClose={() => {
                                setMostrarFormulario(false);
                                setVistaActual('cola');
                            }}
                            matriculaEnfermera={dniEnfermera}
                        />
                    </div>
                )}
            </div>
        </div>
    );
};

export default UrgenciasPage;