import { useState, useRef, useEffect } from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import Input, { TextArea } from '../ui/Input';
import { NivelesEmergencia } from '../../constants/enums';
import pacientesService from '../../services/pacientesService';
import obrasSocialesService from '../../services/obrasSocialesService';
import { mapValidationErrors } from '../../utils/errorUtils';
import './FormularioIngreso.css';
import React from 'react';

const NIVELES_EMERGENCIA = Object.values(NivelesEmergencia);

export const FormularioIngreso = ({ onSubmit, onClose, matriculaEnfermera, showNotification }) => {
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

    const [pacienteData, setPacienteData] = useState({
        nombre: '',
        apellido: '',
        calle: '',
        numero: '',
        localidad: '',
        email: '',
        telefono: '',
        obraSocialId: '',
        numeroAfiliado: '',
        fechaNacimiento: ''
    });

    const [pacienteEncontrado, setPacienteEncontrado] = useState(null);
    const [buscandoPaciente, setBuscandoPaciente] = useState(false);
    const [nombrePacienteDisplay, setNombrePacienteDisplay] = useState('');
    const [mostrarMensajePacienteNuevo, setMostrarMensajePacienteNuevo] = useState(false);
    const [obrasSociales, setObrasSociales] = useState([]);

    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);

    const seccionPacienteNuevoRef = useRef(null);

    useEffect(() => {
        const cargarObrasSociales = async () => {
            try {
                const obras = await obrasSocialesService.obtenerTodas();
                setObrasSociales(obras);
            } catch (error) {
                console.error('Error al cargar obras sociales:', error);
            }
        };
        cargarObrasSociales();
    }, []);

    useEffect(() => {
        if (pacienteEncontrado === false && seccionPacienteNuevoRef.current) {
            setMostrarMensajePacienteNuevo(true);
            setTimeout(() => {
                seccionPacienteNuevoRef.current?.scrollIntoView({
                    behavior: 'smooth',
                    block: 'center'
                });
                // Opcional: hacer focus en el primer campo
                const primerInput = seccionPacienteNuevoRef.current?.querySelector('input');
                primerInput?.focus();
            }, 100);
        }
    }, [pacienteEncontrado]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
        if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));

        if (name === 'cuilPaciente') {
            setPacienteEncontrado(null);
            setNombrePacienteDisplay('');
        }
    };

    const handlePacienteChange = (e) => {
        const { name, value } = e.target;
        setPacienteData(prev => ({ ...prev, [name]: value }));
        // Limpiar error del campo al escribir
        if (errors[name]) setErrors(prev => ({ ...prev, [name]: '' }));
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
                return true;
            } else {
                setPacienteEncontrado(false);
                return false;
            }
        } catch (error) {
            console.error("Error buscando paciente", error);
            setPacienteEncontrado(false);
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
        if (!formData.frecuenciaCardiaca) newErrors.frecuenciaCardiaca = 'La FC es obligatoria';
        if (!formData.frecuenciaRespiratoria) newErrors.frecuenciaRespiratoria = 'La FR es obligatoria';
        if (!formData.frecuenciaSistolica) newErrors.frecuenciaSistolica = 'La FS es obligatoria';
        if (!formData.frecuenciaDiastolica) newErrors.frecuenciaDiastolica = 'La FD es obligatoria';
        if (formData.nivelEmergencia === null) newErrors.nivelEmergencia = 'Seleccione un nivel';

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

            if (existe === false && !mostrarMensajePacienteNuevo) {
                setLoading(false);

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
                const numeroParseado = pacienteData.numero ? parseInt(pacienteData.numero) : null;
                const telefonoParseado = pacienteData.telefono ? parseInt(pacienteData.telefono.replace(/\D/g, '')) : null;
                const obraSocialIdParseado = pacienteData.obraSocialId ? parseInt(pacienteData.obraSocialId) : null;
                const fechaDefault = "2000-01-01";

                datosPacienteNuevo = {
                    cuil: formData.cuilPaciente.trim(),
                    nombre: pacienteData.nombre.trim() || null,
                    apellido: pacienteData.apellido.trim() || null,
                    calle: pacienteData.calle.trim() || null,
                    numero: (numeroParseado && !isNaN(numeroParseado)) ? numeroParseado : null,
                    localidad: pacienteData.localidad.trim() || null,
                    email: pacienteData.email.trim() || null,
                    telefono: (telefonoParseado && !isNaN(telefonoParseado)) ? telefonoParseado : null,
                    obraSocialId: obraSocialIdParseado,
                    numeroAfiliado: pacienteData.numeroAfiliado.trim() || null,
                    fechaNacimiento: pacienteData.fechaNacimiento
                        ? new Date(pacienteData.fechaNacimiento).toISOString()
                        : new Date(fechaDefault).toISOString()
                };
            }
            await onSubmit(urgenciaData, datosPacienteNuevo);

            // Limpiar
            setFormData({
                cuilPaciente: '', informe: '', temperatura: '',
                nivelEmergencia: null, frecuenciaCardiaca: '',
                frecuenciaRespiratoria: '', frecuenciaSistolica: '', frecuenciaDiastolica: ''
            });
            setPacienteEncontrado(null);
            setMostrarMensajePacienteNuevo(false);
            setPacienteData({
                nombre: '', apellido: '', calle: '', numero: '', localidad: '',
                email: '', telefono: '', obraSocialId: '', numeroAfiliado: '',
                fechaNacimiento: ''
            });

        } catch (error) {
            console.error("Error en ingreso:", error);

            if (error.response && error.response.data) {
                // Caso 1: Errores de validación de campos (400 Bad Request con "errors")
                if (error.response.data.errors) {
                    const serverErrors = mapValidationErrors(error.response.data.errors);
                    setErrors(serverErrors);
                    // Opcional: Avisar que hay errores en el formulario
                    if (showNotification) showNotification('Por favor corrija los errores en el formulario', 'error');
                }
                // Caso 2: Errores de Negocio / Conflictos (409 Conflict o 400 con "message")
                // Ej: "El paciente ya está en la cola"
                else if (error.response.data.message) {
                    if (showNotification) {
                        showNotification(error.response.data.message, 'error');
                    } else {
                        setErrors({ informe: error.response.data.message });
                    }
                }
            } else {
                if (showNotification) showNotification('Error de conexión con el servidor', 'error');
            }
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

                    <form onSubmit={handleSubmit} className="formulario-form" noValidate>


                        <div className="form-section">
                            <h3 className="section-title">
                                <span className="section-icon">🆔</span>
                                Identificación del Paciente
                            </h3>
                            <p className="form-section-subtitle">Verificación y datos del paciente</p>

                            <div className="row g-3">
                                <div className="col-12 col-md-6">
                                    <Input
                                        label="CUIL Paciente"
                                        name="cuilPaciente"
                                        value={formData.cuilPaciente}
                                        onChange={handleChange}
                                        placeholder="Ej: 20-12345678-9"
                                        required
                                        error={errors.cuilPaciente}
                                        onBlur={verificarCuil}
                                    />
                                    {buscandoPaciente && <span className="searching-text">Buscando paciente...</span>}
                                    {pacienteEncontrado === true && (
                                        <div className="patient-found-badge">
                                            ✓ Paciente identificado: <strong>{nombrePacienteDisplay}</strong>
                                        </div>
                                    )}
                                </div>

                                <div className="col-12 col-md-6">
                                    <Input
                                        label="DNI Enfermera"
                                        value={matriculaEnfermera}
                                        readOnly
                                        style={{ backgroundColor: '#f3f4f6' }}
                                    />
                                </div>
                            </div>


                            {pacienteEncontrado === false && (
                                <div ref={seccionPacienteNuevoRef} className="new-patient-section fade-in">
                                    <div className="new-patient-header">
                                        <div className="new-patient-icon-wrapper">
                                            <span className="new-patient-icon">👤</span>
                                        </div>
                                        <div className="new-patient-text">
                                            <h4 className="new-patient-title">
                                                Paciente No Registrado
                                                <span className="new-patient-badge">Nuevo</span>
                                            </h4>
                                            <p className="new-patient-description">
                                                Complete los datos del paciente o déjelos en blanco. El sistema usará valores genéricos automáticamente.
                                            </p>
                                        </div>
                                    </div>

                                    <div className="row g-3">
                                        <div className="col-12 col-md-4">
                                            <Input
                                                label="Nombre (Opcional)"
                                                type="nombre"
                                                name="nombre"
                                                value={pacienteData.nombre}
                                                onChange={handlePacienteChange}
                                                placeholder="Sin Registrar"
                                                error={errors.nombre}
                                            />
                                        </div>
                                        <div className="col-12 col-md-4">
                                            <Input
                                                label="Apellido (Opcional)"
                                                name="apellido"
                                                value={pacienteData.apellido}
                                                onChange={handlePacienteChange}
                                                placeholder="Sin Registrar"
                                                error={errors.apellido}
                                            />
                                        </div>
                                        <div className="col-12 col-md-4">
                                            <Input
                                                label="Fecha de Nacimiento"
                                                name="fechaNacimiento"
                                                type="date"
                                                value={pacienteData.fechaNacimiento}
                                                onChange={handlePacienteChange}
                                                error={errors.fechaNacimiento}
                                            />
                                        </div>
                                    </div>

                                    <div className="row g-3">
                                        <div className="col-12 col-md-6">
                                            <Input
                                                label="Email (Opcional)"
                                                name="email"
                                                type="email"
                                                value={pacienteData.email}
                                                onChange={handlePacienteChange}
                                                placeholder="ejemplo@email.com"
                                                error={errors.email}
                                            />
                                        </div>
                                        <div className="col-12 col-md-6">
                                            <Input
                                                label="Teléfono (Opcional)"
                                                name="telefono"
                                                type="tel"
                                                value={pacienteData.telefono}
                                                onChange={handlePacienteChange}
                                                placeholder="3814567890"
                                                error={errors.telefono}
                                            />
                                        </div>
                                    </div>

                                    <div className="row g-3">
                                        <div className="col-12 col-md-5">
                                            <Input
                                                label="Calle (Opcional)"
                                                name="calle"
                                                value={pacienteData.calle}
                                                onChange={handlePacienteChange}
                                                placeholder="San Martin"
                                                error={errors.calle}
                                            />
                                        </div>
                                        <div className="col-12 col-md-3">
                                            <Input
                                                label="Número (Opcional)"
                                                name="numero"
                                                type="number"
                                                value={pacienteData.numero}
                                                onChange={handlePacienteChange}
                                                placeholder="999"
                                                error={errors.numero}
                                            />
                                        </div>
                                        <div className="col-12 col-md-4">
                                            <Input
                                                label="Localidad"
                                                name="localidad"
                                                value={pacienteData.localidad}
                                                onChange={handlePacienteChange}
                                                placeholder="Tucumán"
                                                error={errors.localidad}
                                            />
                                        </div>
                                    </div>

                                    <div className="row g-3">
                                        <div className="col-12 col-md-6">
                                            <label className="input-label">Obra Social (Opcional)</label>
                                            <select
                                                name="obraSocialId"
                                                value={pacienteData.obraSocialId}
                                                onChange={handlePacienteChange}
                                                className="form-select"
                                            >
                                                <option value="">Sin Obra Social</option>
                                                {obrasSociales.map(obra => (
                                                    <option key={obra.id} value={obra.id}>
                                                        {obra.nombre}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>
                                        {pacienteData.obraSocialId && (
                                            <div className="col-12 col-md-6">
                                                <Input
                                                    label="Número de Afiliado"
                                                    name="numeroAfiliado"
                                                    value={pacienteData.numeroAfiliado}
                                                    onChange={handlePacienteChange}
                                                    placeholder="123456"
                                                    required={!!pacienteData.obraSocialId}
                                                    error={errors.numeroAfiliado}
                                                />
                                            </div>
                                        )}
                                    </div>
                                </div>
                            )}
                        </div>

                        <div className="form-section">
                            <h3 className="section-title">
                                <span className="section-icon">🚨</span>
                                Nivel de Emergencia
                            </h3>
                            <p className="form-section-subtitle">Seleccione el nivel de prioridad según triage</p>
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

                        <div className="form-section">
                            <h3 className="section-title">
                                <span className="section-icon">🩺</span>
                                Signos Vitales
                            </h3>
                            <p className="form-section-subtitle">Mediciones iniciales del paciente</p>
                            <div className="row g-3">
                                <div className="col-12 col-md-4">
                                    <Input
                                        label="Temp (°C)"
                                        placeholder="36.5"
                                        name="temperatura"
                                        type="number"
                                        step="0.1"
                                        value={formData.temperatura}
                                        onChange={handleChange}
                                        required
                                        error={errors.temperatura} />
                                </div>
                                <div className="col-12 col-md-4">
                                    <Input
                                        label="FC (lpm)"
                                        placeholder="80"
                                        name="frecuenciaCardiaca"
                                        type="number"
                                        value={formData.frecuenciaCardiaca}
                                        onChange={handleChange}
                                        required
                                        error={errors.frecuenciaCardiaca} />
                                </div>
                                <div className="col-12 col-md-4">
                                    <Input
                                        label="FR (rpm)"
                                        placeholder="16"
                                        name="frecuenciaRespiratoria"
                                        type="number"
                                        value={formData.frecuenciaRespiratoria}
                                        onChange={handleChange}
                                        required
                                        error={errors.frecuenciaRespiratoria} />
                                </div>
                            </div>
                            <div className="row g-3">
                                <div className="col-12 col-md-6">
                                    <Input
                                        label="TA Sistólica"
                                        placeholder="120"
                                        name="frecuenciaSistolica"
                                        type="number"
                                        value={formData.frecuenciaSistolica}
                                        onChange={handleChange}
                                        required
                                        error={errors.frecuenciaSistolica} />
                                </div>
                                <div className="col-12 col-md-6">
                                    <Input
                                        label="TA Diastólica"
                                        placeholder="80"
                                        name="frecuenciaDiastolica"
                                        type="number"
                                        value={formData.frecuenciaDiastolica}
                                        onChange={handleChange}
                                        required
                                        error={errors.frecuenciaDiastolica} />
                                </div>
                            </div>
                        </div>

                        <div className="form-section">
                            <h3 className="section-title">
                                <span className="section-icon">📝</span>
                                Informe de Ingreso
                            </h3>
                            <p className="form-section-subtitle">Motivo de consulta, síntomas y observaciones iniciales</p>
                            <TextArea
                                name="informe"
                                placeholder="Describa el motivo de consulta, síntomas principales y cualquier observación relevante..."
                                value={formData.informe}
                                onChange={handleChange}
                                required
                                error={errors.informe}
                                rows={6}
                            />
                            <div className="helper-text" style={{ fontSize: '13px', color: '#6b7280', marginTop: '8px', padding: '12px', background: '#f0f9ff', borderLeft: '3px solid #3b82f6', borderRadius: '4px' }}>
                                💡 Incluya: motivo de consulta, síntomas principales, duración de síntomas y cualquier antecedente relevante.
                            </div>
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