import { useState } from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import { TextArea } from '../ui/Input';
import Badge from '../ui/Badge';
import { getNivelEmergenciaInfo } from '../../constants/enums';
import { formatCuil } from '../../utils/cuilHelper';
import { mapValidationErrors } from '../../utils/errorUtils';
import './ModalAtencionMedica.css';
import React from 'react';

export const ModalAtencionMedica = ({ ingreso, onSubmit, onClose }) => {
    const [informeMedico, setInformeMedico] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    if (!ingreso) return null;

    const nivelInfo = getNivelEmergenciaInfo(ingreso.nivelEmergencia);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!informeMedico.trim() || informeMedico.trim().length < 10) {
            setError('El informe médico debe tener al menos 10 caracteres');
            return;
        }

        setLoading(true);
        try {
            await onSubmit(ingreso.cuilPaciente, informeMedico);
        } catch (err) {
            if (err.response && err.response.data && err.response.data.errors) {
                const mappedErrors = mapValidationErrors(err.response.data.errors);
                if (mappedErrors.informeMedico) {
                    setError(mappedErrors.informeMedico);
                } else {
                    setError('Error de validación en el servidor');
                }
            } else {
                setError(err.message || 'Error al registrar atención');
            }
        } finally {
            setLoading(false);
        }
    };

    const formatearFecha = (fecha) => {
        return new Date(fecha).toLocaleString('es-AR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    return (
        <div className="modal-overlay">
            <div className="modal-container">
                <Card className="modal-card">
                    <div className="modal-header">
                        <div>
                            <h2 className="modal-title">Registrar Atención Médica</h2>
                            <p className="modal-subtitle">
                                Paciente: <strong>{ingreso.nombrePaciente} {ingreso.apellidoPaciente}</strong>
                            </p>
                        </div>
                        <button className="btn-close" onClick={onClose}>&times;</button>
                    </div>

                    <div className="ingreso-resumen">
                        <h3 className="seccion-titulo">📋 Datos del Ingreso</h3>

                        <div className="info-grid">
                            <div className="info-item">
                                <span className="info-label">CUIL:</span>
                                <span className="resumen-value">{formatCuil(ingreso.cuilPaciente)}</span>
                            </div>

                            <div className="info-item">
                                <span className="info-label">Fecha Ingreso:</span>
                                <span className="info-value">{formatearFecha(ingreso.fechaIngreso)}</span>
                            </div>

                            <div className="info-item">
                                <span className="info-label">Nivel Emergencia:</span>
                                <Badge color={nivelInfo.color}>{nivelInfo.label}</Badge>
                            </div>
                        </div>

                        <div className="signos-vitales-resumen">
                            <h4 className="subseccion-titulo">Signos Vitales</h4>
                            <div className="signos-grid">
                                <div className="signo-item">
                                    <span className="signo-icono">🌡️</span>
                                    <div>
                                        <div className="signo-label">Temperatura</div>
                                        <div className="signo-valor">{ingreso.signosVitales.temperatura}°C</div>
                                    </div>
                                </div>

                                <div className="signo-item">
                                    <span className="signo-icono">💓</span>
                                    <div>
                                        <div className="signo-label">FC</div>
                                        <div className="signo-valor">{ingreso.signosVitales.frecuenciaCardiaca} lpm</div>
                                    </div>
                                </div>

                                <div className="signo-item">
                                    <span className="signo-icono">🫁</span>
                                    <div>
                                        <div className="signo-label">FR</div>
                                        <div className="signo-valor">{ingreso.signosVitales.frecuenciaRespiratoria} rpm</div>
                                    </div>
                                </div>

                                <div className="signo-item">
                                    <span className="signo-icono">🩺</span>
                                    <div>
                                        <div className="signo-label">TA</div>
                                        <div className="signo-valor">
                                            {ingreso.signosVitales.tensionSistolica}/{ingreso.signosVitales.tensionDiastolica} mmHg
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="informe-enfermera-seccion">
                            <h4 className="subseccion-titulo">📝 Informe Inicial (Enfermera)</h4>
                            <div className="informe-enfermera-contenido">
                                {ingreso.informeInicial}
                            </div>
                        </div>
                    </div>

                    <form onSubmit={handleSubmit} className="atencion-form">
                        <div className="atencion-seccion">
                            <h3 className="seccion-titulo">📝 Informe Médico</h3>

                            <TextArea
                                label="Diagnóstico y Tratamiento"
                                value={informeMedico}
                                onChange={(e) => {
                                    setInformeMedico(e.target.value);
                                    setError('');
                                }}
                                placeholder="Diagnóstico, tratamiento, observaciones, derivaciones..."
                                rows={8}
                                required
                                error={error}
                            />

                            <div className="helper-text">
                                💡 Incluya: diagnóstico, tratamiento administrado, evolución esperada y derivaciones si corresponde.
                            </div>
                        </div>

                        <div className="modal-actions">
                            <Button variant="secondary" type="button" onClick={onClose} disabled={loading}>
                                Cancelar
                            </Button>
                            <Button type="submit" disabled={loading} fullWidth>
                                {loading ? 'Registrando...' : 'Registrar Atención y Finalizar'}
                            </Button>
                        </div>
                    </form>
                </Card>
            </div>
        </div>
    );
};

export default ModalAtencionMedica;
