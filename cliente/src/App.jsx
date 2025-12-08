import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import UrgenciasPage from './pages/UrgenciasPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import './App.css';

// Componente simple para proteger rutas
const PrivateRoute = ({ children }) => {
    const token = localStorage.getItem('authToken');
    return token ? children : <Navigate to="/login" replace />;
};

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route
                    path="/urgencias"
                    element={
                        <PrivateRoute>
                            <UrgenciasPage />
                        </PrivateRoute>
                    }
                />
                <Route path="/" element={<Navigate to="/urgencias" replace />} />
            </Routes>
        </Router>
    );
}

export default App;