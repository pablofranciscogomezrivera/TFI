import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom'; // Para logout si quieres
import DashboardUrgencias from '../components/urgencias/DashboardUrgencias';
import ColaPrioridad from '../components/urgencias/ColaPrioridad';
import FormularioIngreso from '../components/urgencias/FormularioIngreso';
import Button from '../components/ui/Button';
import urgenciasService from '../services/urgenciasService';
import './UrgenciasPage.css';
import React from 'react';

export const UrgenciasPage = () => {
    const [pacientes, setPacientes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [mostrarFormulario, setMostrarFormulario] = useState(false);
    const [vistaActual, setVistaActual] = useState('cola');
    const dniEnfermera = localStorage.getItem('profesionalDNI') || 'Sin Identificar';
    
    // 0: Medico, 1: Enfermera 
    const userRole = parseInt(localStorage.getItem('userRole'));
    const esMedico = userRole === 0;
    const esEnfermera = userRole === 1;
    // --------------------------------------

    const navigate = useNavigate();

    // Matrículas hardcodeadas por ahora (esto lo toma el interceptor api.js también)
    const matriculaEnfermera = '87654321';
    const matriculaDoctor = 'MP12345';

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
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        cargarPacientes();
        const interval = setInterval(cargarPacientes, 30000);
        return () => clearInterval(interval);
    }, []);

    const handleRegistrarIngreso = async (data) => {
        try {
            await urgenciasService.registrarUrgencia(data, matriculaEnfermera);
            alert('Paciente registrado exitosamente');
            await cargarPacientes();
            setMostrarFormulario(false);
            setVistaActual('cola');
        } catch (err) {
            console.error('Error al registrar ingreso:', err);
            alert('Error al registrar el paciente');
        }
    };

    const handleReclamarPaciente = async () => {
        try {
            const paciente = await urgenciasService.reclamarPaciente(matriculaDoctor);

            // Mostrar modal o alerta con el paciente asignado
            alert(`PACIENTE ASIGNADO:\n\n${paciente.nombrePaciente} ${paciente.apellidoPaciente}\n\nInforme: ${paciente.informeInicial}`);

            // Recargar la lista (el paciente debería desaparecer de la cola)
            await cargarPacientes();
        } catch (err) {
            console.error(err);
            alert(err.response?.data?.message || 'No hay pacientes en espera o error al reclamar.');
        }
    };
    // -----------------------------------------------------

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('userRole');
        navigate('/login');
    };

    return (
        <div className="urgencias-page">
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
                        onClick={() => {
                            setVistaActual('cola');
                            setMostrarFormulario(false);
                        }}
                    >
                        Cola de Prioridad
                    </button>

                    {/* Solo Enfermera ve el botón de Nuevo Ingreso */}
                    {esEnfermera && (
                        <button
                            className={`tab-button ${vistaActual === 'formulario' ? 'tab-button-active' : ''}`}
                            onClick={() => {
                                setVistaActual('formulario');
                                setMostrarFormulario(true);
                            }}
                        >
                            Nuevo Ingreso
                        </button>
                    )}
                </div>

                {vistaActual === 'cola' && (
                    <>
                        <DashboardUrgencias pacientes={pacientes} />

                        {/* --- NUEVO: Botón de Acción para Médico --- */}
                        {esMedico && (
                            <div style={{ marginBottom: '20px', padding: '20px', background: '#e0f2fe', borderRadius: '8px', border: '1px solid #bae6fd', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                <div>
                                    <h3 style={{ margin: 0, color: '#0369a1' }}>Panel Medico</h3>
                                    <p style={{ margin: 0, color: '#0c4a6e' }}>Hay {pacientes.length} pacientes esperando atencion.</p>
                                </div>
                                <Button onClick={handleReclamarPaciente} size="large">
                                    Llamar Siguiente Paciente
                                </Button>
                            </div>
                        )}
                        {/* ----------------------------------------- */}

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
                            onSubmit={handleRegistrarIngreso}
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