import React from 'react'
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import UrgenciasPage from './pages/UrgenciasPage';
import './App.css';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Navigate to="/urgencias" replace />} />
                <Route path="/urgencias" element={<UrgenciasPage />} />
            </Routes>
        </Router>
    );
}

export default App;