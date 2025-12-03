import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import DashboardUrgencias from '../components/urgencias/DashboardUrgencias';
import ColaPrioridad from '../components/urgencias/ColaPrioridad';
import FormularioIngreso from '../components/urgencias/FormularioIngreso';
import ModalAtencionMedica from '../components/urgencias/ModalAtencionMedica';
import Button from '../components/ui/Button';
import urgenciasService from '../services/urgenciasService';
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

    // Estados para el modal de atención médica
    const [mostrarModalAtencion, setMostrarModalAtencion] = useState(false);
    const [ingresoSeleccionado, setIngresoSeleccionado] = useState(null);

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

            // Cargar pacientes pendientes
            const dataPendientes = await urgenciasService.obtenerListaEspera();
            setPacientes(dataPendientes);

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
            // Combinar los datos de urgencia con los datos opcionales del paciente
            const datosCompletos = {
                ...urgenciaData,
                // Si hay datos del paciente nuevo, agregarlos
                nombrePaciente: pacienteNuevoData?.nombre || null,
                apellidoPaciente: pacienteNuevoData?.apellido || null,
                callePaciente: pacienteNuevoData?.calle || null,
                numeroPaciente: pacienteNuevoData?.numero || null,
                localidadPaciente: pacienteNuevoData?.localidad || null,
                emailPaciente: pacienteNuevoData?.email || null,
                telefonoPaciente: pacienteNuevoData?.telefono || null,
                obraSocialIdPaciente: pacienteNuevoData?.obraSocialId || null,
                numeroAfiliadoPaciente: pacienteNuevoData?.numeroAfiliado || null,
                fechaNacimientoPaciente: pacienteNuevoData?.fechaNacimiento || null
            };

            // Debug: Ver qué se está enviando
            console.log('Datos a enviar:', JSON.stringify(datosCompletos, null, 2));

            // Registrar la urgencia (el backend creará el paciente automáticamente si no existe)
            await urgenciasService.registrarUrgencia(datosCompletos, dniEnfermera);

            const mensaje = pacienteNuevoData
                ? 'Ingreso registrado exitosamente. Paciente creado automáticamente.'
                : 'Ingreso registrado exitosamente.';

            showNotification(mensaje, 'success');
            await cargarPacientes();

            // Volver a la cola
            setMostrarFormulario(false);
            setVistaActual('cola');

        } catch (err) {
            console.error('Error completo:', err);
            console.error('Response data:', err.response?.data);
            console.error('Response status:', err.response?.status);

            // Mostrar errores de validación de FluentValidation
            if (err.response?.data?.errors) {
                console.error('❌ ERRORES DE VALIDACIÓN:', err.response.data.errors);

                // Construir mensaje con todos los errores
                const errores = Object.entries(err.response.data.errors)
                    .map(([campo, mensajes]) => `${campo}: ${mensajes.join(', ')}`)
                    .join('\n');

                showNotification(`Errores de validación:\n${errores}`, 'error');
            } else {
                const msg = err.response?.data?.message || err.message || 'Error al procesar el ingreso.';
                showNotification(msg, 'error');
            }
        }
    };

    const iniciarNuevoIngreso = () => {
        setVistaActual('formulario');
        setMostrarFormulario(true);
    };

    const handleReclamarPaciente = async () => {
        try {
            // Ya NO se pasa la matrícula - el backend la obtiene del JWT
            const ingresoReclamado = await urgenciasService.reclamarPaciente();

            // Recargar lista de pacientes en espera
            await cargarPacientes();

            // Abrir directamente el modal de atención con el paciente reclamado
            setIngresoSeleccionado(ingresoReclamado);
            setMostrarModalAtencion(true);

            showNotification(`Paciente asignado: ${ingresoReclamado.nombrePaciente} ${ingresoReclamado.apellidoPaciente}`, 'success');
        } catch (err) {
            const msg = err.response?.data?.message || 'No hay pacientes en espera.';
            showNotification(msg, 'info');
        }
    };


    const handleCerrarModalAtencion = () => {
        setMostrarModalAtencion(false);
        setIngresoSeleccionado(null);
    };

    const handleRegistrarAtencion = async (cuilPaciente, informeMedico) => {
        try {
            // Ya NO se pasa la matrícula - el backend la obtiene del JWT
            await urgenciasService.registrarAtencion(cuilPaciente, informeMedico);

            showNotification('Atención registrada exitosamente. Paciente dado de alta.', 'success');

            // Cerrar modal y recargar datos
            handleCerrarModalAtencion();
            await cargarPacientes();
        } catch (err) {
            const msg = err.response?.data?.message || 'Error al registrar la atención';
            throw new Error(msg);
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
                            <>
                                <div style={{ marginBottom: '20px', padding: '20px', background: '#e0f2fe', borderRadius: '8px', border: '1px solid #bae6fd', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                    <div>
                                        <h3 style={{ margin: 0, color: '#0369a1' }}>Panel Médico</h3>
                                        <p style={{ margin: 0, color: '#0c4a6e' }}>
                                            {pacientes.length > 0
                                                ? `${pacientes.length} paciente${pacientes.length !== 1 ? 's' : ''} esperando atención`
                                                : 'No hay pacientes en espera'}
                                        </p>
                                    </div>
                                    <Button
                                        onClick={handleReclamarPaciente}
                                        size="large"
                                        disabled={pacientes.length === 0}
                                    >
                                        Llamar Siguiente Paciente
                                    </Button>
                                </div>


                            </>
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
                            onSubmit={handleRegistrarIngresoCompleto}
                            onClose={() => {
                                setMostrarFormulario(false);
                                setVistaActual('cola');
                            }}
                            matriculaEnfermera={dniEnfermera}
                        />
                    </div>
                )}
            </div>

            {/* Modal de Atención Médica */}
            {mostrarModalAtencion && ingresoSeleccionado && (
                <ModalAtencionMedica
                    ingreso={ingresoSeleccionado}
                    onSubmit={handleRegistrarAtencion}
                    onClose={handleCerrarModalAtencion}
                />
            )}
        </div>
    );
};

export default UrgenciasPage;