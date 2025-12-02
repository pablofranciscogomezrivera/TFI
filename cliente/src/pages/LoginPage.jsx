import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import Card from '../components/ui/Card';
import Input from '../components/ui/Input';
import Button from '../components/ui/Button';
import Notification from '../components/ui/Notification'; // Importar
import './LoginPage.css';

const LoginPage = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [notification, setNotification] = useState(null); // Estado para notificación
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const showNotification = (message, type) => {
        setNotification({ message, type });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setNotification(null); // Limpiar anteriores

        try {
            const response = await api.post('/auth/login', { email, password });

            localStorage.setItem('authToken', response.data.token);
            localStorage.setItem('userRole', response.data.usuario.tipoAutoridad);

            const prof = response.data.profesional;
            if (prof) {
                localStorage.setItem('matricula', prof.matricula);
                localStorage.setItem('profesionalNombre', `${prof.nombre} ${prof.apellido}`);
                localStorage.setItem('profesionalDNI', prof.dni);
            }

            // Mensaje de éxito
            showNotification(`¡Bienvenido/a, ${prof ? prof.nombre : 'Usuario'}! Accediendo...`, 'success');

            
            setTimeout(() => {
                navigate('/urgencias');
            }, 1500);

        } catch (err) {
            console.error(err);
            // Mensajes de error personalizados
            if (err.response && err.response.status === 401) {
                showNotification('Credenciales incorrectas. Verifique su email y contraseña.', 'error');
            } else {
                showNotification('Error de conexión con el servidor. Intente más tarde.', 'error');
            }
            setLoading(false); 
        }
    };

    return (
        <div className="login-container">
            {/* Componente de Notificación */}
            {notification && (
                <Notification
                    message={notification.message}
                    type={notification.type}
                    onClose={() => setNotification(null)}
                />
            )}

            <Card className="login-card">
                <div className="login-header">
                    <h2 className="login-title">Hospital Urgencias</h2>
                    <p className="login-subtitle">Sistema de Gestión de Guardia</p>
                </div>

                <form onSubmit={handleSubmit} className="login-form">
                    <Input
                        label="Email"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="usuario@hospital.com"
                        required
                        disabled={loading}
                    />

                    <Input
                        label={"Contraseña"}
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="••••••••"
                        required
                        disabled={loading}
                    />

                    <div className="login-actions">
                        <Button type="submit" fullWidth disabled={loading}>
                            {loading ? 'Validando...' : 'Iniciar Sesión'}
                        </Button>
                    </div>
                </form>
            </Card>
        </div>
    );
};

export default LoginPage;