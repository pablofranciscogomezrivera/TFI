import { useState } from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import Input from '../ui/Input';
import './FormularioNuevoPaciente.css';

export const FormularioNuevoPaciente = ({ onSubmit, onCancel, cuilInicial = '' }) => {
    const [formData, setFormData] = useState({
        cuil: cuilInicial,
        nombre: '',
        apellido: '',
        calle: '',
        numero: '',
        localidad: '',
        obraSocialId: '',
        numeroAfiliado: '',
    });

    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        if (errors[name]) {
            setErrors(prev => ({
                ...prev,
                [name]: ''
            }));
        }
    };

    const validateForm = () => {
        const newErrors = {};

        if (!formData.cuil.trim()) {
            newErrors.cuil = 'El CUIL es obligatorio';
        }

        if (!formData.nombre.trim()) {
            newErrors.nombre = 'El nombre es obligatorio';
        }

        if (!formData.apellido.trim()) {
            newErrors.apellido = 'El apellido es obligatorio';
        }

        if (!formData.calle.trim()) {
            newErrors.calle = 'La calle es obligatoria';
        }

        if (!formData.numero) {
            newErrors.numero = 'El numero es obligatorio';
        }

        if (!formData.localidad.trim()) {
            newErrors.localidad = 'La localidad es obligatoria';
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
                cuil: formData.cuil.trim(),
                nombre: formData.nombre.trim(),
                apellido: formData.apellido.trim(),
                calle: formData.calle.trim(),
                numero: parseInt(formData.numero),
                localidad: formData.localidad.trim(),
            };

            // Solo agregar obra social si se proporcionó
            if (formData.obraSocialId && formData.numeroAfiliado) {
                dataToSubmit.obraSocialId = parseInt(formData.obraSocialId);
                dataToSubmit.numeroAfiliado = formData.numeroAfiliado.trim();
            }

            await onSubmit(dataToSubmit);
        } catch (error) {
            console.error('Error al registrar paciente:', error);
            throw error;
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="formulario-paciente-overlay">
            <div className="formulario-paciente-container">
                <Card className="formulario-paciente-card">
                    <div className="formulario-paciente-header">
                        <div>
                            <h2 className="formulario-paciente-title">Registrar Nuevo Paciente</h2>
                            <p className="formulario-paciente-subtitle">
                                El paciente no existe en el sistema. Complete los datos para crearlo.
                            </p>
                        </div>
                    </div>

                    <form onSubmit={handleSubmit} className="formulario-paciente-form">
                        <div className="form-section">
                            <h3 className="section-title">Datos Personales</h3>

                            <Input
                                label="CUIL"
                                name="cuil"
                                value={formData.cuil}
                                onChange={handleChange}
                                placeholder="20-30123456-3"
                                required
                                error={errors.cuil}
                                disabled={!!cuilInicial}
                            />

                            <div className="form-row">
                                <Input
                                    label="Nombre"
                                    name="nombre"
                                    value={formData.nombre}
                                    onChange={handleChange}
                                    placeholder="Juan"
                                    required
                                    error={errors.nombre}
                                />
                                <Input
                                    label="Apellido"
                                    name="apellido"
                                    value={formData.apellido}
                                    onChange={handleChange}
                                    placeholder="Perez"
                                    required
                                    error={errors.apellido}
                                />
                            </div>
                        </div>

                        <div className="form-section">
                            <h3 className="section-title">Domicilio</h3>

                            <div className="form-row">
                                <Input
                                    label="Calle"
                                    name="calle"
                                    value={formData.calle}
                                    onChange={handleChange}
                                    placeholder="San Martín"
                                    required
                                    error={errors.calle}
                                />
                                <Input
                                    label="Numero"
                                    name="numero"
                                    type="number"
                                    value={formData.numero}
                                    onChange={handleChange}
                                    placeholder="123"
                                    required
                                    error={errors.numero}
                                />
                            </div>

                            <Input
                                label="Localidad"
                                name="localidad"
                                value={formData.localidad}
                                onChange={handleChange}
                                placeholder="San Miguel de Tucuman"
                                required
                                error={errors.localidad}
                            />
                        </div>

                        <div className="form-section">
                            <h3 className="section-title">Obra Social (Opcional)</h3>

                            <div className="form-row">
                                <Input
                                    label="ID Obra Social"
                                    name="obraSocialId"
                                    type="number"
                                    value={formData.obraSocialId}
                                    onChange={handleChange}
                                    placeholder="1"
                                    error={errors.obraSocialId}
                                />
                                <Input
                                    label="Número de Afiliado"
                                    name="numeroAfiliado"
                                    value={formData.numeroAfiliado}
                                    onChange={handleChange}
                                    placeholder="12345"
                                    error={errors.numeroAfiliado}
                                />
                            </div>
                        </div>

                        <div className="form-actions">
                            <Button
                                type="button"
                                variant="secondary"
                                onClick={onCancel}
                                disabled={loading}
                            >
                                Cancelar
                            </Button>
                            <Button
                                type="submit"
                                variant="primary"
                                disabled={loading}
                            >
                                {loading ? 'Registrando...' : 'Registrar y Continuar'}
                            </Button>
                        </div>
                    </form>
                </Card>
            </div>
        </div>
    );
};

export default FormularioNuevoPaciente;