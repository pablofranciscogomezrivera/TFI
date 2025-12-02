import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import Card from '../components/ui/Card';
import Input from '../components/ui/Input';
import Button from '../components/ui/Button';
import './LoginPage.css'; 

const LoginPage = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const response = await api.post('/auth/login', { email, password });

            localStorage.setItem('authToken', response.data.token);
            // Asegúrate de guardar el rol si lo necesitas para redirigir
            localStorage.setItem('userRole', response.data.usuario.tipoAutoridad); 

            const prof = response.data.profesional;
            if (prof) {
                localStorage.setItem('matricula', prof.matricula);
                localStorage.setItem('profesionalNombre', `${prof.nombre} ${prof.apellido}`);
                localStorage.setItem('profesionalDNI', prof.dni);
            }

            navigate('/urgencias');
        } catch (err) {
            console.error(err);
            setError('Credenciales invalidas. Por favor, intente nuevamente.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-container">
            <Card className="login-card">
                <div className="login-header">
                    <h2 className="login-title">Bienvenido</h2>
                    <p className="login-subtitle">Ingrese sus credenciales para continuar</p>
                </div>

                <form onSubmit={handleSubmit} className="login-form">
                    <Input
                        label="Email"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="nombre@hospital.com"
                        required
                        disabled={loading}
                    />

                    <Input
                        label="Contraseña"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="••••••••"
                        required
                        disabled={loading}
                    />

                    {error && <div className="login-error">{error}</div>}

                    <div className="login-actions">
                        <Button type="submit" fullWidth disabled={loading}>
                            {loading ? 'Ingresando...' : 'Iniciar Sesion'}
                        </Button>
                    </div>
                </form>
            </Card>
        </div>
    );
};

export default LoginPage;