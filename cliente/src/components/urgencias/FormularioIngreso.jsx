import { useState } from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import Input, { TextArea } from '../ui/Input';
import { NivelesEmergencia } from '../../constants/enums';
import pacientesService from '../../services/pacientesService'; // Importamos el servicio aquí
import './FormularioIngreso.css';
import React from 'react';

const NIVELES_EMERGENCIA = Object.values(NivelesEmergencia);

export const FormularioIngreso = ({ onSubmit, onClose, matriculaEnfermera }) => {
    // Estado de Urgencia
    const [formData, setFormData] = useState({
        cuilPaciente: '',
        informe: '',
        temperatura: '',
        nivelEmergencia: null,
        frecuenciaCardiaca: '',
        frecuenciaRespiratoria: '',
        frecuenciaSistolica: '',
        frecuenciaDiastolica: '',
    });

    // Estado para Paciente Nuevo (Campos Opcionales)
    const [pacienteData, setPacienteData] = useState({
        nombre: '',
        apellido: '',
        calle: '',
        numero: '',
        localidad: ''
    });

    // Estados de Control
    const [pacienteEncontrado, setPacienteEncontrado] = useState(null); // null: sin buscar, true: existe, false: nuevo
    const [buscandoPaciente, setBuscandoPaciente] = useState(false);
    const [nombrePacienteDisplay, setNombrePacienteDisplay] = useState(''); // Para mostrar si ya existe

    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);

    // Manejadores de cambios
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
        if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));

        // Si cambia el CUIL, reseteamos el estado de búsqueda
        if (name === 'cuilPaciente') {
            setPacienteEncontrado(null);
            setNombrePacienteDisplay('');
        }
    };

    const handlePacienteChange = (e) => {
        const { name, value } = e.target;
        setPacienteData(prev => ({ ...prev, [name]: value }));
    };

    const handleNivelSelect = (nivelId) => {
        setFormData(prev => ({ ...prev, nivelEmergencia: nivelId }));
        if (errors.nivelEmergencia) setErrors(prev => ({ ...prev, nivelEmergencia: '' }));
    };

    const realizarVerificacionCuil = async (cuil) => {
        if (!cuil || cuil.length < 10) return null;

        setBuscandoPaciente(true);
        try {
            const paciente = await pacientesService.buscarPaciente(cuil);
            if (paciente) {
                setPacienteEncontrado(true);
                setNombrePacienteDisplay(`${paciente.nombre} ${paciente.apellido}`);
                return true; // Encontrado
            } else {
                setPacienteEncontrado(false);
                return false; // No encontrado (Nuevo)
            }
        } catch (error) {
            console.error("Error buscando paciente", error);
            setPacienteEncontrado(false); // Asumimos nuevo si falla
            return false;
        } finally {
            setBuscandoPaciente(false);
        }
    };

    const verificarCuil = async () => {
        if (pacienteEncontrado === null) {
            realizarVerificacionCuil(formData.cuilPaciente.trim());
        }
    };

    const validateForm = () => {
        const newErrors = {};
        if (!formData.cuilPaciente.trim()) newErrors.cuilPaciente = 'El CUIL es obligatorio';
        if (!formData.informe.trim()) newErrors.informe = 'El informe es obligatorio';
        if (!formData.temperatura) newErrors.temperatura = 'La temperatura es obligatoria';
        if (!formData.frecuenciaCardiaca) newErrors.frecuenciaCardiaca = 'Obligatorio';
        if (!formData.frecuenciaRespiratoria) newErrors.frecuenciaRespiratoria = 'Obligatorio';
        if (!formData.frecuenciaSistolica) newErrors.frecuenciaSistolica = 'Obligatorio';
        if (!formData.frecuenciaDiastolica) newErrors.frecuenciaDiastolica = 'Obligatorio';
        if (formData.nivelEmergencia === null) newErrors.nivelEmergencia = 'Seleccione un nivel';

        // Validar negativos
        if (parseFloat(formData.frecuenciaCardiaca) < 0) newErrors.frecuenciaCardiaca = 'No negativo';
        if (parseFloat(formData.frecuenciaRespiratoria) < 0) newErrors.frecuenciaRespiratoria = 'No negativo';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!validateForm()) return;

        setLoading(true);

        try {
            let existe = pacienteEncontrado;

            if (existe === null) {
                existe = await realizarVerificacionCuil(formData.cuilPaciente.trim());
            }

            if (existe === null) {
                setLoading(false);
                return;
            }

             /*CASO ESPECIAL: Es paciente nuevo y el usuario recién lo descubre
             Si acabamos de descubrir que es nuevo (estaba en null y pasó a false),
             detenemos el envío para mostrarle los campos opcionales.*/
            if (existe === false && pacienteEncontrado === null) {
                setLoading(false);
                alert("Paciente nuevo detectado. Complete los datos opcionales si lo desea o confirme nuevamente.");
                return;
            }
            // 1. Preparar datos de Urgencia
            const urgenciaData = {
                cuilPaciente: formData.cuilPaciente.trim(),
                informe: formData.informe.trim(),
                // Usamos || 0 para evitar NaN, aunque la validación previa debería atajarlo
                temperatura: parseFloat(formData.temperatura) || 0,
                nivelEmergencia: parseInt(formData.nivelEmergencia),
                frecuenciaCardiaca: parseFloat(formData.frecuenciaCardiaca) || 0,
                frecuenciaRespiratoria: parseFloat(formData.frecuenciaRespiratoria) || 0,
                frecuenciaSistolica: parseFloat(formData.frecuenciaSistolica) || 0,
                frecuenciaDiastolica: parseFloat(formData.frecuenciaDiastolica) || 0,
            };

            // 2. Preparar datos de Paciente (Solo si es nuevo)
            let datosPacienteNuevo = null;
            if (existe === false) {
                datosPacienteNuevo = {
                    cuil: formData.cuilPaciente.trim(),
                    nombre: pacienteData.nombre.trim() || "Sin Registrar",
                    apellido: pacienteData.apellido.trim() || "Sin Registrar",
                    calle: pacienteData.calle.trim() || "Sin Registrar",
                    numero: pacienteData.numero ? parseInt(pacienteData.numero) : 999,
                    localidad: pacienteData.localidad.trim() || "San Miguel de Tucuman",
                    fechaNacimiento: new Date("1900-01-01").toISOString()
                };
            }
            // 3. Enviar todo al padre (UrgenciasPage)
            await onSubmit(urgenciaData, datosPacienteNuevo);

            // Limpiar
            setFormData({
                cuilPaciente: '', informe: '', temperatura: '',
                nivelEmergencia: null, frecuenciaCardiaca: '',
                frecuenciaRespiratoria: '', frecuenciaSistolica: '', frecuenciaDiastolica: ''
            });
            setPacienteEncontrado(null);
            setPacienteData({ nombre: '', apellido: '', calle: '', numero: '', localidad: '' });

        } catch (error) {
            console.error(error);
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
                            <h2 className="formulario-title">Nuevo Ingreso</h2>
                            <p className="formulario-subtitle">Ingrese CUIL para verificar paciente</p>
                        </div>
                        <button className="btn-close" onClick={onClose}>&times;</button>
                    </div>

                    <form onSubmit={handleSubmit} className="formulario-form">

                        {/* --- SECCIÓN 1: IDENTIFICACIÓN Y PACIENTE --- */}
                        <div className="form-section">
                            <div className="form-row">
                                <div className="input-group">
                                    <Input
                                        label="CUIL Paciente"
                                        name="cuilPaciente"
                                        value={formData.cuilPaciente}
                                        onChange={handleChange}
                                        placeholder="Ej: 20-12345678-9"
                                        required
                                        error={errors.cuilPaciente}
                                        // Al salir del foco, buscamos al paciente
                                        onBlur={verificarCuil}
                                    />
                                    {buscandoPaciente && <span className="searching-text">Buscando paciente...</span>}

                                    {/* Feedback Visual: Paciente Encontrado */}
                                    {pacienteEncontrado === true && (
                                        <div className="patient-found-badge">
                                            ✓ Paciente identificado: <strong>{nombrePacienteDisplay}</strong>
                                        </div>
                                    )}
                                </div>

                                <Input
                                    label="DNI Enfermera"
                                    value={matriculaEnfermera}
                                    readOnly
                                    style={{ backgroundColor: '#f3f4f6' }}
                                />
                            </div>

                            {/* CAMPOS OPCIONALES DE PACIENTE (Solo si NO existe) */}
                            {pacienteEncontrado === false && (
                                <div className="new-patient-section fade-in">
                                    <div className="new-patient-header">
                                        <span className="warning-icon">⚠️</span>
                                        <p>Paciente no registrado. Complete los datos o deje en blanco para usar genéricos.</p>
                                    </div>

                                    <div className="form-row">
                                        <Input label="Nombre (Opcional)" name="nombre" value={pacienteData.nombre} onChange={handlePacienteChange} placeholder="Sin Registrar" />
                                        <Input label="Apellido (Opcional)" name="apellido" value={pacienteData.apellido} onChange={handlePacienteChange} placeholder="Sin Registrar" />
                                    </div>
                                    <div className="form-row three-cols">
                                        <Input label="Calle (Opcional)" name="calle" value={pacienteData.calle} onChange={handlePacienteChange} placeholder = "San Martin" />
                                        <Input label="Número (Opcional)" name="numero" type="number" value={pacienteData.numero} onChange={handlePacienteChange} placeholder="999" />
                                        <Input label="Localidad" name="localidad" value={pacienteData.localidad} onChange={handlePacienteChange} placeholder="Tucumán" />
                                    </div>
                                </div>
                            )}
                        </div>

                        {/* --- SECCIÓN 2: TRIAGE (Niveles) --- */}
                        <div className="form-section">
                            <h3>Nivel de Emergencia</h3>
                            <div className="niveles-grid">
                                {NIVELES_EMERGENCIA.map((nivel) => (
                                    <div
                                        key={nivel.id}
                                        className={`nivel-item ${nivel.color ? nivel.color.toLowerCase() : ''} ${formData.nivelEmergencia === nivel.id ? 'selected' : ''}`}
                                        onClick={() => handleNivelSelect(nivel.id)}
                                    >
                                        <span className="nivel-nombre">{nivel.nombre}</span>
                                        <span className="nivel-tiempo">{nivel.tiempoEspera}</span>
                                    </div>
                                ))}
                            </div>
                            {errors.nivelEmergencia && <p className="error-text">{errors.nivelEmergencia}</p>}
                        </div>

                        {/* --- SECCIÓN 3: SIGNOS VITALES --- */}
                        <div className="form-section">
                            <h3>Signos Vitales</h3>
                            <div className="form-row">
                                <Input label="Temp (°C)" placeholder = "36.5" name="temperatura" type="number" step="0.1" value={formData.temperatura} onChange={handleChange} required error={errors.temperatura} />
                                <Input label="FC (lpm)" placeholder="80"  name="frecuenciaCardiaca" type="number" value={formData.frecuenciaCardiaca} onChange={handleChange} required error={errors.frecuenciaCardiaca} />
                                <Input label="FR (rpm)" placeholder="16" name="frecuenciaRespiratoria" type="number" value={formData.frecuenciaRespiratoria} onChange={handleChange} required error={errors.frecuenciaRespiratoria} />
                            </div>
                            <div className="form-row">
                                <Input label="TA Sistólica" placeholder="120" name="frecuenciaSistolica" type="number" value={formData.frecuenciaSistolica} onChange={handleChange} required error={errors.frecuenciaSistolica} />
                                <Input label="TA Diastólica" placeholder="80" name="frecuenciaDiastolica" type="number" value={formData.frecuenciaDiastolica} onChange={handleChange} required error={errors.frecuenciaDiastolica} />
                            </div>
                        </div>

                        {/* --- SECCIÓN 4: INFORME --- */}
                        <div className="form-section">
                            <TextArea
                                label="Informe de Ingreso"
                                name="informe"
                                placeholder="Informe de Ingreso..."
                                value={formData.informe}
                                onChange={handleChange}
                                required
                                error={errors.informe}
                            />
                        </div>

                        <div className="form-actions">
                            <Button variant="secondary" type="button" onClick={onClose} disabled={loading}>Cancelar</Button>
                            <Button type="submit" disabled={loading} fullWidth>
                                {loading ? 'Procesando...' : 'Confirmar Ingreso'}
                            </Button>
                        </div>
                    </form>
                </Card>
            </div>
        </div>
    );
};

export default FormularioIngreso;