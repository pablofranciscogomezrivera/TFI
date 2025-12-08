import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import api from '../services/api';
import Card from '../components/ui/Card';
import Input from '../components/ui/Input';
import Button from '../components/ui/Button';
import Notification from '../components/ui/Notification';
import { debugToken } from '../utils/jwtHelper';
import './LoginPage.css';

const LoginPage = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [notification, setNotification] = useState(null); 
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    const showNotification = (message, type) => {
        setNotification({ message, type });
    };

    // Detectar si viene de una sesión expirada
    useEffect(() => {
        const reason = searchParams.get('reason');
        if (reason === 'session_expired') {
            setTimeout(() => {
                setNotification({
                    message: 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente.',
                    type: 'error'
                });
            }, 0);
        }
    }, [searchParams]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setNotification(null);

        try {
            const response = await api.post('/auth/login', { email, password });

            console.log('Login Response:', response.data);
            console.log('Token recibido:', response.data.token);
            console.log('Usuario:', response.data.usuario);

            localStorage.setItem('authToken', response.data.token);
            localStorage.setItem('userRole', response.data.usuario.tipoAutoridad);

            const prof = response.data.profesional;
            if (prof) {
                localStorage.setItem('matricula', prof.matricula);
                localStorage.setItem('profesionalNombre', `${prof.nombre} ${prof.apellido}`);
                localStorage.setItem('profesionalDNI', prof.dni);
            }

            console.log('🔍 Verificando claims del JWT:');
            debugToken();

            showNotification(`¡Bienvenido/a, ${prof ? prof.nombre : 'Usuario'}! Accediendo...`, 'success');


            setTimeout(() => {
                navigate('/urgencias');
            }, 1500);

        } catch (err) {
            console.error(err);
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

                    <div style={{ textAlign: 'center', marginTop: '16px' }}>
                        <span style={{ color: '#64748b', fontSize: '14px', fontFamily: 'inherit' }}>
                            ¿No tienes cuenta?{' '}
                            <a
                                href="/register"
                                style={{
                                    color: '#3b82f6',
                                    textDecoration: 'none',
                                    fontWeight: '500',
                                    fontFamily: 'inherit'
                                }}
                            >
                                Registrarse
                            </a>
                        </span>
                    </div>
                </form>
            </Card>
        </div>
    );
};

export default LoginPage;