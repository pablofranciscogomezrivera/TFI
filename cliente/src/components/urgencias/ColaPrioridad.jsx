import { formatDistanceToNow } from 'date-fns';
import { es } from 'date-fns/locale';
import Card from '../ui/Card';
import Badge from '../ui/Badge';
import { getBadgeVariantByEmergency, getEmergencyLabel } from '../../utils/emergencyLevels';
import './ColaPrioridad.css';
import React from 'react';


const formatearCuil = (cuil) => {
    if (!cuil) return '';
    // Si ya tiene guiones, lo devolvemos igual
    if (cuil.includes('-')) return cuil;
    // Si es un número limpio de 11 dígitos, le ponemos guiones
    if (cuil.length === 11) {
        return `${cuil.substring(0, 2)}-${cuil.substring(2, 10)}-${cuil.substring(10)}`;
    }
    return cuil;
};

export const ColaPrioridad = ({ pacientes, onActualizar }) => {
    const formatearTiempo = (fecha) => {
        try {
            return `hace ${formatDistanceToNow(new Date(fecha), { locale: es })}`;
        } catch {
            return 'hace un momento';
        }
    };

    if (!pacientes || pacientes.length === 0) {
        return (
            <div className="cola-vacia">
                <svg className="cola-vacia-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <p className="cola-vacia-text">No hay pacientes en espera</p>
            </div>
        );
    }

    return (
        <div className="cola-prioridad">
            <div className="cola-header">
                <div>
                    <h2 className="cola-title">Cola de Prioridad</h2>
                    <p className="cola-subtitle">Pacientes ordenados por nivel de emergencia</p>
                </div>
                <button className="btn-actualizar" onClick={onActualizar}>
                    <svg className="actualizar-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                    </svg>
                    Actualizar
                </button>
            </div>

            <div className="cola-info">
                <p className="cola-count">{pacientes.length} paciente{pacientes.length !== 1 ? 's' : ''} en espera</p>
            </div>

            <div className="cola-lista">
                {pacientes.map((paciente, index) => (
                    <Card key={`${paciente.cuilPaciente}-${index}`} className="paciente-card">
                        <div className="paciente-header">
                            <div className="paciente-info">
                                <div className="paciente-posicion">#{index + 1}</div>
                                <div className="paciente-datos">
                                    <h3 className="paciente-nombre">
                                        {paciente.nombrePaciente} {paciente.apellidoPaciente}
                                    </h3>
                                    <p className="paciente-dni">CUIL: {formatearCuil(paciente.cuilPaciente)}</p>
                                </div>
                            </div>
                            <Badge variant={getBadgeVariantByEmergency(paciente.nivelEmergencia)}>
                                {getEmergencyLabel(paciente.nivelEmergencia)}
                            </Badge>
                        </div>

                        <div className="paciente-tiempo">
                            <svg className="tiempo-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <span>{formatearTiempo(paciente.fechaIngreso)}</span>
                        </div>

                        {paciente.signosVitales && (
                            <div className="paciente-vitales">
                                <div className="vital-item">
                                    <span className="vital-label">Temp:</span>
                                    <span className="vital-value">{paciente.signosVitales.temperatura} C</span>
                                </div>
                                <div className="vital-item">
                                    <span className="vital-label">FC:</span>
                                    <span className="vital-value">{paciente.signosVitales.frecuenciaCardiaca} lpm</span>
                                </div>
                                <div className="vital-item">
                                    <span className="vital-label">FR:</span>
                                    <span className="vital-value">{paciente.signosVitales.frecuenciaRespiratoria} rpm</span>
                                </div>
                                <div className="vital-item">
                                    <span className="vital-label">TA:</span>
                                    <span className="vital-value">
                                        {paciente.signosVitales.tensionSistolica}/{paciente.signosVitales.tensionDiastolica} mmHg
                                    </span>
                                </div>
                            </div>
                        )}
                    </Card>
                ))}
            </div>
        </div>
    );
};

export default ColaPrioridad;