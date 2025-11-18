import { useState } from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import Input, { TextArea } from '../ui/Input';
import { NivelEmergencia, getNivelEmergenciaInfo } from '../../constants/enums';
import './FormularioIngreso.css';
import React from 'react';

const NIVELES_EMERGENCIA = [
    { value: NivelEmergencia.CRITICA, ...getNivelEmergenciaInfo(NivelEmergencia.CRITICA) },
    { value: NivelEmergencia.EMERGENCIA, ...getNivelEmergenciaInfo(NivelEmergencia.EMERGENCIA) },
    { value: NivelEmergencia.URGENCIA, ...getNivelEmergenciaInfo(NivelEmergencia.URGENCIA) },
    { value: NivelEmergencia.URGENCIA_MENOR, ...getNivelEmergenciaInfo(NivelEmergencia.URGENCIA_MENOR) },
    { value: NivelEmergencia.SIN_URGENCIA, ...getNivelEmergenciaInfo(NivelEmergencia.SIN_URGENCIA) },
];

export const FormularioIngreso = ({ onSubmit, onClose, matriculaEnfermera }) => {
    const [formData, setFormData] = useState({
        cuilPaciente: '',
        informe: '',
        temperatura: '',
        nivelEmergencia: NivelEmergencia.URGENCIA, // Urgencia por defecto
        frecuenciaCardiaca: '',
        frecuenciaRespiratoria: '',
        frecuenciaSistolica: '',
        frecuenciaDiastolica: '',
    });

    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        // Limpiar error del campo al modificarlo
        if (errors[name]) {
            setErrors(prev => ({
                ...prev,
                [name]: ''
            }));
        }
    };

    const validateForm = () => {
        const newErrors = {};

        // Validar CUIL
        if (!formData.cuilPaciente.trim()) {
            newErrors.cuilPaciente = 'El CUIL del paciente es obligatorio';
        }

        // Validar informe
        if (!formData.informe.trim()) {
            newErrors.informe = 'El informe medico es obligatorio';
        }

        // Validar frecuencia cardíaca
        if (!formData.frecuenciaCardiaca) {
            newErrors.frecuenciaCardiaca = 'La frecuencia cardiaca es obligatoria';
        } else if (parseFloat(formData.frecuenciaCardiaca) < 0) {
            newErrors.frecuenciaCardiaca = 'La frecuencia cardiaca no puede ser negativa';
        }

        // Validar frecuencia respiratoria
        if (!formData.frecuenciaRespiratoria) {
            newErrors.frecuenciaRespiratoria = 'La frecuencia respiratoria es obligatoria';
        } else if (parseFloat(formData.frecuenciaRespiratoria) < 0) {
            newErrors.frecuenciaRespiratoria = 'La frecuencia respiratoria no puede ser negativa';
        }

        // Validar frecuencia sistólica
        if (!formData.frecuenciaSistolica) {
            newErrors.frecuenciaSistolica = 'La frecuencia sistolica es obligatoria';
        } else if (parseFloat(formData.frecuenciaSistolica) < 0) {
            newErrors.frecuenciaSistolica = 'La frecuencia sistolica no puede ser negativa';
        }

        // Validar frecuencia diastólica
        if (!formData.frecuenciaDiastolica) {
            newErrors.frecuenciaDiastolica = 'La frecuencia diastolica es obligatoria';
        } else if (parseFloat(formData.frecuenciaDiastolica) < 0) {
            newErrors.frecuenciaDiastolica = 'La frecuencia diastolica no puede ser negativa';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!validateForm()) {
            return;
        }

        setLoading(true);

        try {
            const dataToSubmit = {
                cuilPaciente: formData.cuilPaciente.trim(),
                informe: formData.informe.trim(),
                temperatura: formData.temperatura ? parseFloat(formData.temperatura) : null,
                nivelEmergencia: parseInt(formData.nivelEmergencia),
                frecuenciaCardiaca: parseFloat(formData.frecuenciaCardiaca),
                frecuenciaRespiratoria: parseFloat(formData.frecuenciaRespiratoria),
                frecuenciaSistolica: parseFloat(formData.frecuenciaSistolica),
                frecuenciaDiastolica: parseFloat(formData.frecuenciaDiastolica),
            };

            await onSubmit(dataToSubmit);

            // Limpiar formulario después del éxito
            setFormData({
                cuilPaciente: '',
                informe: '',
                temperatura: '',
                nivelEmergencia: NivelEmergencia.URGENCIA,
                frecuenciaCardiaca: '',
                frecuenciaRespiratoria: '',
                frecuenciaSistolica: '',
                frecuenciaDiastolica: '',
            });
        } catch (error) {
            console.error('Error al registrar ingreso:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="formulario-overlay">
            <div className="formulario-container">
                <Card className="formulario-card">
                    <div className="formulario-header">
                        <div>
                            <h2 className="formulario-title">Registrar Nuevo Paciente</h2>
                            <p className="formulario-subtitle">
                                Complete el formulario para agregar un paciente a la cola de prioridad
                            </p>
                        </div>
                        {onClose && (
                            <button className="btn-close" onClick={onClose}>
                                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            </button>
                        )}
                    </div>

                    <form onSubmit={handleSubmit} className="formulario-form">
                        <div className="form-section">
                            <h3 className="section-title">
                                <svg className="section-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                                </svg>
                                Nuevo Ingreso de Paciente
                            </h3>

                            <div className="form-row">
                                <Input
                                    label="DNI del Paciente"
                                    name="cuilPaciente"
                                    value={formData.cuilPaciente}
                                    onChange={handleChange}
                                    placeholder="12345678"
                                    required
                                    error={errors.cuilPaciente}
                                />
                                <Input
                                    label="DNI de Enfermera"
                                    name="matriculaEnfermera"
                                    value={matriculaEnfermera}
                                    disabled
                                    placeholder="87654321"
                                />
                            </div>

                            <TextArea
                                label="Informe Medico"
                                name="informe"
                                value={formData.informe}
                                onChange={handleChange}
                                placeholder="Describa los sintomas y motivo de consulta..."
                                required
                                error={errors.informe}
                                rows={4}
                            />
                        </div>

                        <div className="form-section">
                            <h3 className="section-title">Nivel de Emergencia</h3>
                            <div className="niveles-emergencia">
                                {NIVELES_EMERGENCIA.map((nivel) => (
                                    <label
                                        key={nivel.value}
                                        className={`nivel-option ${formData.nivelEmergencia === nivel.value ? 'nivel-option-active' : ''}`}
                                        style={{
                                            borderColor: formData.nivelEmergencia === nivel.value ? nivel.color : '#e5e7eb'
                                        }}
                                    >
                                        <input
                                            type="radio"
                                            name="nivelEmergencia"
                                            value={nivel.value}
                                            checked={formData.nivelEmergencia === nivel.value}
                                            onChange={handleChange}
                                            className="nivel-input"
                                        />
                                        <div
                                            className="nivel-color"
                                            style={{ backgroundColor: nivel.color }}
                                        />
                                        <div className="nivel-info">
                                            <div className="nivel-label">{nivel.label}</div>
                                            <div className="nivel-tiempo">{nivel.tiempo}</div>
                                        </div>
                                    </label>
                                ))}
                            </div>
                        </div>

                        <div className="form-section">
                            <h3 className="section-title">Signos Vitales</h3>

                            <div className="form-row">
                                <Input
                                    label="Temperatura (C)"
                                    name="temperatura"
                                    type="number"
                                    step="0.1"
                                    value={formData.temperatura}
                                    onChange={handleChange}
                                    placeholder="36.5"
                                    error={errors.temperatura}
                                />
                                <Input
                                    label="Frecuencia Cardiaca (lpm)"
                                    name="frecuenciaCardiaca"
                                    type="number"
                                    value={formData.frecuenciaCardiaca}
                                    onChange={handleChange}
                                    placeholder="75"
                                    required
                                    error={errors.frecuenciaCardiaca}
                                />
                            </div>

                            <div className="form-row">
                                <Input
                                    label="Frecuencia Respiratoria (rpm)"
                                    name="frecuenciaRespiratoria"
                                    type="number"
                                    value={formData.frecuenciaRespiratoria}
                                    onChange={handleChange}
                                    placeholder="16"
                                    required
                                    error={errors.frecuenciaRespiratoria}
                                />
                            </div>

                            <div className="form-section-subtitle">Tension Arterial (mmHg)</div>
                            <div className="form-row">
                                <Input
                                    label="Sistolica"
                                    name="frecuenciaSistolica"
                                    type="number"
                                    value={formData.frecuenciaSistolica}
                                    onChange={handleChange}
                                    placeholder="120"
                                    required
                                    error={errors.frecuenciaSistolica}
                                />
                                <Input
                                    label="Diastolica"
                                    name="frecuenciaDiastolica"
                                    type="number"
                                    value={formData.frecuenciaDiastolica}
                                    onChange={handleChange}
                                    placeholder="80"
                                    required
                                    error={errors.frecuenciaDiastolica}
                                />
                            </div>
                        </div>

                        <div className="form-actions">
                            {onClose && (
                                <Button
                                    type="button"
                                    variant="secondary"
                                    onClick={onClose}
                                    disabled={loading}
                                >
                                    Cancelar
                                </Button>
                            )}
                            <Button
                                type="submit"
                                variant="primary"
                                disabled={loading}
                                fullWidth={!onClose}
                                icon={
                                    <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" width="20" height="20">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                                    </svg>
                                }
                            >
                                {loading ? 'Registrando...' : 'Registrar Paciente'}
                            </Button>
                        </div>
                    </form>
                </Card>
            </div>
        </div>
    );
};

export default FormularioIngreso;