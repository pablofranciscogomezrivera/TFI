import { useMemo } from 'react';
import Card from '../ui/Card';
import { NivelEmergencia } from '../../constants/enums';
import './DashboardUrgencias.css';
import React from 'react';

export const DashboardUrgencias = ({ pacientes }) => {
    const stats = useMemo(() => {
        if (!pacientes) {
            return {
                pacientesEspera: 0,
                casosCriticos: 0,
                emergencias: 0,
            };
        }

        return {
            pacientesEspera: pacientes.length,
            casosCriticos: pacientes.filter(p => p.nivelEmergencia === NivelEmergencia.CRITICA).length,
            emergencias: pacientes.filter(p => p.nivelEmergencia === NivelEmergencia.EMERGENCIA).length,
        };
    }, [pacientes]);

    return (
        <div className="dashboard-container">
            <div className="dashboard-stats">
                <Card className="stat-card">
                    <div className="card-header">
                        <span className="card-title">
                            <svg className="card-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                            </svg>
                            Pacientes en Espera
                        </span>
                    </div>
                    <div className="card-value">{stats.pacientesEspera}</div>
                    <div className="card-subtitle">En cola de prioridad</div>
                </Card>

                <Card className="stat-card stat-card-critical">
                    <div className="card-header">
                        <span className="card-title">
                            <svg className="card-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
                            </svg>
                            Casos Criticos
                        </span>
                    </div>
                    <div className="card-value card-value-critical">{stats.casosCriticos}</div>
                    <div className="card-subtitle">Requieren atencion inmediata</div>
                </Card>

                <Card className="stat-card stat-card-emergency">
                    <div className="card-header">
                        <span className="card-title">
                            <svg className="card-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                            </svg>
                            Emergencias
                        </span>
                    </div>
                    <div className="card-value card-value-emergency">{stats.emergencias}</div>
                    <div className="card-subtitle">Casos de emergencia</div>
                </Card>
            </div>
        </div>
    );
};

export default DashboardUrgencias;