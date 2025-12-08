/ ***********************************************************
// This example support/e2e.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands';

// Alternatively you can use CommonJS syntax:
// require('./commands')

// Desactivar verificación de fetch para axios
Cypress.on('uncaught:exception', (err, runnable) => {
    // returning false here prevents Cypress from failing the test
    // Útil para errores de red que no son críticos
    if (err.message.includes('fetch') || err.message.includes('Network')) {
        return false;
    }
    return true;
});

// Comando para limpiar la base de datos
Cypress.Commands.add('limpiarBaseDatos', () => {
    // Aquí podrías agregar lógica para limpiar la base de datos
    // Por ahora trabajamos con el estado actual
    cy.log('Base de datos en memoria - no requiere limpieza');
});

// Comando para registrar urgencia vía API
Cypress.Commands.add('registrarUrgencia', (urgencia) => {
    const apiUrl = Cypress.env('apiUrl');
    return cy.request({
        method: 'POST',
        url: `${apiUrl}/urgencias`,
        headers: {
            'X-Enfermera-Matricula': 'ENF123'
        },
        body: urgencia
    });
});
