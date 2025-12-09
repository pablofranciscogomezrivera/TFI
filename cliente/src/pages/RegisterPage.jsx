import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import Card from '../components/ui/Card';
import Input from '../components/ui/Input';
import Button from '../components/ui/Button';
import Notification from '../components/ui/Notification';
import { validateCuilFormat } from '../utils/cuilHelper';
import { mapValidationErrors } from '../utils/errorUtils';
import './LoginPage.css';
import './RegisterPage.css';

const RegisterPage = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
        confirmPassword: '',
        nombre: '',
        apellido: '',
        dni: '',
        cuil: '',
        matricula: '',
        tipoAutoridad: 0, // 0 = Medico, 1 = Enfermera
        fechaNacimiento: '',
        telefono: ''
    });
    const [notification, setNotification] = useState(null);
    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const showNotification = (message, type) => {
        setNotification({ message, type });
    };

    const handleChange = (e) => {
        const { name, value } = e.target;

        if (name === 'cuil') {
            // Validar solo formato básico (con o sin guiones)
            // El backend validará el dígito verificador
            if (value && !validateCuilFormat(value)) {
                setErrors(prev => ({
                    ...prev,
                    cuil: 'Formato inválido. Debe ser 11 dígitos (ej: 20-12345678-9 o 20123456789)'
                }));
            } else {
                setErrors(prev => ({ ...prev, cuil: '' }));
            }
        } else {
            // Limpiar error del campo al escribir
            if (errors[name]) {
                setErrors(prev => ({ ...prev, [name]: '' }));
            }
        }

        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setNotification(null);

        // Validación manual de campos requeridos
        const newErrors = {};
        if (!formData.email) newErrors.email = 'El email es obligatorio';
        if (!formData.password) newErrors.password = 'La contraseña es obligatoria';
        if (!formData.nombre) newErrors.nombre = 'El nombre es obligatorio';
        if (!formData.apellido) newErrors.apellido = 'El apellido es obligatorio';
        if (!formData.dni) newErrors.dni = 'El DNI es obligatorio';
        if (!formData.cuil) newErrors.cuil = 'El CUIL es obligatorio';
        if (!formData.matricula) newErrors.matricula = 'La matrícula es obligatoria';

        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            showNotification('Por favor, complete todos los campos obligatorios', 'error');
            setLoading(false);
            return;
        }

        if (!validateCuilFormat(formData.cuil)) {
            showNotification('CUIL inválido. Por favor, ingrese 11 dígitos válidos', 'error');
            setErrors(prev => ({
                ...prev,
                cuil: 'Formato inválido. Debe ser 11 dígitos (ej: 20-12345678-9 o 20123456789)'
            }));
            setLoading(false);
            return;
        }

        if (formData.password !== formData.confirmPassword) {
            showNotification('Las contraseñas no coinciden', 'error');
            setLoading(false);
            return;
        }

        try {
            const requestData = {
                email: formData.email,
                password: formData.password,
                nombre: formData.nombre,
                apellido: formData.apellido,
                dni: parseInt(formData.dni),
                cuil: formData.cuil,
                matricula: formData.matricula,
                tipoAutoridad: parseInt(formData.tipoAutoridad),
                fechaNacimiento: formData.fechaNacimiento || null,
                telefono: formData.telefono ? parseInt(formData.telefono) : null
            };

            await api.post('/auth/registrar', requestData);

            showNotification('Usuario registrado exitosamente. Redirigiendo al login...', 'success');

            setTimeout(() => {
                navigate('/login');
            }, 2000);

        } catch (err) {
            console.error('Error completo:', err);
            console.log('Response data:', err.response?.data);

            if (err.response && err.response.data) {
                const backendErrors = err.response.data.errors;
                const errorMsg = err.response.data.message || err.response.data.title || '';

                console.log('Backend errors object:', backendErrors);
                console.log('Backend errors keys:', backendErrors ? Object.keys(backendErrors) : 'N/A');

                // Si hay errores de validación del backend
                if (backendErrors && typeof backendErrors === 'object' && Object.keys(backendErrors).length > 0) {
                    // Mapear errores usando la utilidad
                    const mappedErrors = mapValidationErrors(backendErrors);
                    console.log('Mapped errors:', mappedErrors);

                    // Buscar cualquier key que CONTENGA "cuil" (case-insensitive) en los errores originales
                    const cuilKey = Object.keys(backendErrors).find(key =>
                        key.toLowerCase().includes('cuil')
                    );
                    console.log('CUIL key found:', cuilKey);

                    if (cuilKey && !mappedErrors.cuil) {
                        // Si encontramos una key de CUIL pero no se mapeó correctamente
                        const cuilError = Array.isArray(backendErrors[cuilKey])
                            ? backendErrors[cuilKey][0]
                            : backendErrors[cuilKey];
                        mappedErrors.cuil = cuilError;
                        console.log('CUIL error manually assigned:', cuilError);
                    }

                    console.log('Final errors to set:', mappedErrors);
                    setErrors(mappedErrors);

                    // Mostrar notificación indicando que revise los campos
                    showNotification('Por favor, revise los campos marcados con error.', 'error');
                } else if (errorMsg.toLowerCase().includes('cuil')) {
                    // Caso backup: el mensaje principal menciona CUIL
                    setErrors(prev => ({
                        ...prev,
                        cuil: errorMsg
                    }));
                    showNotification('Por favor, revise el campo CUIL.', 'error');
                } else {
                    // Error genérico del backend
                    showNotification(errorMsg || 'Error al registrar usuario', 'error');
                }
            } else {
                showNotification('Error de conexión con el servidor. Intente más tarde.', 'error');
            }
            setLoading(false);
        }
    };

    return (
        <div className="login-container register-page">
            {notification && (
                <Notification
                    message={notification.message}
                    type={notification.type}
                    onClose={() => setNotification(null)}
                />
            )}

            <Card className="login-card">
                <div className="login-header">
                    <h2 className="login-title">Registro de Usuario</h2>
                    <p className="login-subtitle">Complete el formulario para crear su cuenta</p>
                </div>

                <form onSubmit={handleSubmit} className="register-form" noValidate>
                    <div className="form-section">
                        <h3 className="section-title">Tipo de Empleado</h3>
                        <div className="row g-3">
                            <div className="col-12">
                                <select
                                    name="tipoAutoridad"
                                    value={formData.tipoAutoridad}
                                    onChange={handleChange}
                                    required
                                    disabled={loading}
                                    className="form-select"
                                >
                                    <option value={0}>Médico/a</option>
                                    <option value={1}>Enfermero/a</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div className="form-section">
                        <h3 className="section-title">Credenciales</h3>
                        <div className="row g-3">
                            <div className="col-12">
                                <Input
                                    label="Email"
                                    type="email"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    placeholder="usuario@hospital.com"
                                    required
                                    disabled={loading}
                                    error={errors.email}
                                />
                            </div>
                        </div>
                        <div className="row g-3">
                            <div className="col-12 col-md-6">
                                <Input
                                    label="Contraseña"
                                    type="password"
                                    name="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    placeholder="Mínimo 8 caracteres"
                                    required
                                    disabled={loading}
                                    error={errors.password}
                                />
                            </div>
                            <div className="col-12 col-md-6">
                                <Input
                                    label="Confirmar Contraseña"
                                    type="password"
                                    name="confirmPassword"
                                    value={formData.confirmPassword}
                                    onChange={handleChange}
                                    placeholder="Repetir contraseña"
                                    required
                                    disabled={loading}
                                />
                            </div>
                        </div>
                    </div>

                    <div className="form-section">
                        <h3 className="section-title">Datos Personales</h3>
                        <div className="row g-3">
                            <div className="col-12 col-md-6">
                                <Input
                                    label="Nombre"
                                    type="text"
                                    name="nombre"
                                    value={formData.nombre}
                                    onChange={handleChange}
                                    placeholder="Juan"
                                    required
                                    disabled={loading}
                                    error={errors.nombre}
                                />
                            </div>
                            <div className="col-12 col-md-6">
                                <Input
                                    label="Apellido"
                                    type="text"
                                    name="apellido"
                                    value={formData.apellido}
                                    onChange={handleChange}
                                    placeholder="Pérez"
                                    required
                                    disabled={loading}
                                    error={errors.apellido}
                                />
                            </div>
                        </div>
                        <div className="row g-3">
                            <div className="col-12 col-md-6">
                                <Input
                                    label="DNI"
                                    type="number"
                                    name="dni"
                                    value={formData.dni}
                                    onChange={handleChange}
                                    placeholder="12345678"
                                    required
                                    disabled={loading}
                                    error={errors.dni}
                                />
                            </div>
                            <div className="col-12 col-md-6">
                                <Input
                                    label="CUIL"
                                    type="text"
                                    name="cuil"
                                    value={formData.cuil}
                                    onChange={handleChange}
                                    placeholder="20-12345678-9"
                                    required
                                    disabled={loading}
                                    error={errors.cuil}
                                />
                            </div>
                        </div>
                        <div className="row g-3">
                            <div className="col-12">
                                <Input
                                    label="Matrícula Profesional"
                                    type="text"
                                    name="matricula"
                                    value={formData.matricula}
                                    onChange={handleChange}
                                    placeholder="MED-12345 o ENF-67890"
                                    required
                                    disabled={loading}
                                    error={errors.matricula}
                                />
                            </div>
                        </div>
                    </div>

                    <div className="form-section optional-section">
                        <h3 className="section-title">Información Adicional (Opcional)</h3>
                        <div className="row g-3">
                            <div className="col-12 col-md-6">
                                <Input
                                    label="Fecha de Nacimiento"
                                    type="date"
                                    name="fechaNacimiento"
                                    value={formData.fechaNacimiento}
                                    onChange={handleChange}
                                    disabled={loading}
                                    error={errors.fechaNacimiento}
                                />
                            </div>
                            <div className="col-12 col-md-6">
                                <Input
                                    label="Teléfono"
                                    type="tel"
                                    name="telefono"
                                    value={formData.telefono}
                                    onChange={handleChange}
                                    placeholder="3814567890"
                                    disabled={loading}
                                    error={errors.telefono}
                                />
                            </div>
                        </div>
                    </div>

                    <div className="form-actions">
                        <Button
                            type="button"
                            variant="secondary"
                            onClick={() => navigate('/login')}
                            disabled={loading}
                        >
                            Cancelar
                        </Button>
                        <Button type="submit" disabled={loading}>
                            {loading ? 'Registrando...' : 'Registrar Usuario'}
                        </Button>
                    </div>

                    <div style={{ textAlign: 'center', marginTop: '24px', paddingTop: '20px', borderTop: '1px solid #e5e7eb' }}>
                        <span style={{ color: '#64748b', fontSize: '14px', fontFamily: 'inherit' }}>
                            ¿Ya tienes cuenta?{' '}
                            <a
                                href="/login"
                                style={{
                                    color: '#3b82f6',
                                    textDecoration: 'none',
                                    fontWeight: '500',
                                    fontFamily: 'inherit'
                                }}
                            >
                                Iniciar Sesión
                            </a>
                        </span>
                    </div>
                </form>
            </Card>
        </div>
    );
};

export default RegisterPage;