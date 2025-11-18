import { useState, useEffect } from 'react';
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
    const [vistaActual, setVistaActual] = useState('cola'); // 'cola' o 'formulario'

    // Matrícula de enfermera (esto debería venir de un contexto de autenticación)
    const matriculaEnfermera = '87654321';

    const cargarPacientes = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await urgenciasService.obtenerListaEspera();
            setPacientes(data);
        } catch (err) {
            console.error('Error al cargar pacientes:', err);
            setError('No se pudo cargar la lista de pacientes');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        cargarPacientes();

        // Actualizar cada 30 segundos
        const interval = setInterval(cargarPacientes, 30000);

        return () => clearInterval(interval);
    }, []);

    const handleRegistrarIngreso = async (data) => {
        try {
            await urgenciasService.registrarUrgencia(data, matriculaEnfermera);

            // Mostrar mensaje de éxito
            alert('Paciente registrado exitosamente');

            // Recargar lista de pacientes
            await cargarPacientes();

            // Cerrar formulario y volver a la cola
            setMostrarFormulario(false);
            setVistaActual('cola');
        } catch (err) {
            console.error('Error al registrar ingreso:', err);
            const errorMsg = err.response?.data?.error || 'Error al registrar el paciente';
            alert(errorMsg);
            throw err; // Re-lanzar para que el formulario maneje el estado de carga
        }
    };

    return (
        <div className="urgencias-page">
            <header className="urgencias-header">
                <div className="header-content">
                    <div>
                        <h1 className="page-title">Modulo de Urgencias</h1>
                        <p className="page-subtitle">Sistema de gestion de admisiones de emergencia</p>
                    </div>
                </div>
            </header>

            <div className="urgencias-content">
                {/* Navegación de pestañas */}
                <div className="tabs-navigation">
                    <button
                        className={`tab-button ${vistaActual === 'cola' ? 'tab-button-active' : ''}`}
                        onClick={() => {
                            setVistaActual('cola');
                            setMostrarFormulario(false);
                        }}
                    >
                        <svg className="tab-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                        </svg>
                        Cola de Prioridad
                    </button>
                    <button
                        className={`tab-button ${vistaActual === 'formulario' ? 'tab-button-active' : ''}`}
                        onClick={() => {
                            setVistaActual('formulario');
                            setMostrarFormulario(true);
                        }}
                    >
                        <svg className="tab-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                        </svg>
                        Nuevo Ingreso
                    </button>
                </div>

                {/* Contenido principal */}
                {vistaActual === 'cola' && (
                    <>
                        <DashboardUrgencias pacientes={pacientes} />

                        {loading && !pacientes.length ? (
                            <div className="loading-container">
                                <div className="loading-spinner"></div>
                                <p>Cargando pacientes...</p>
                            </div>
                        ) : error ? (
                            <div className="error-container">
                                <p className="error-message">{error}</p>
                                <Button onClick={cargarPacientes}>Reintentar</Button>
                            </div>
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
                            matriculaEnfermera={matriculaEnfermera}
                        />
                    </div>
                )}
            </div>
        </div>
    );
};

export default UrgenciasPage;