/// <reference types="cypress" />

/**
 * Pruebas E2E para el Módulo de Urgencias
 * Valida la integración entre frontend y backend
 */

describe('Módulo de Urgencias - Integración Frontend-Backend', () => {
    const apiUrl = Cypress.env('apiUrl');
    const matriculaEnfermera = 'ENF123';

    beforeEach(() => {
        // Limpiar el estado antes de cada prueba
        cy.request('GET', `${apiUrl}/urgencias/lista-espera`).then((response) => {
            // Si hay pacientes en la lista, podríamos limpiarlos aquí
            // Por ahora trabajamos con el estado actual
        });

        // Visitar la página principal
        cy.visit('/urgencias');
    });

    describe('Vista de Cola de Prioridad', () => {
        it('Debe mostrar el dashboard con estadísticas', () => {
            cy.contains('Módulo de Urgencias').should('be.visible');
            cy.contains('Pacientes en Espera').should('be.visible');
            cy.contains('Casos Críticos').should('be.visible');
            cy.contains('Emergencias').should('be.visible');
        });

        it('Debe mostrar la cola de prioridad', () => {
            cy.contains('Cola de Prioridad').should('be.visible');
            cy.contains('Actualizar').should('be.visible');
        });

        it('Debe actualizar la lista al hacer clic en Actualizar', () => {
            cy.contains('Actualizar').click();
            // Esperar a que se complete la petición
            cy.wait(1000);
            // Verificar que sigue mostrando el contenido
            cy.contains('Cola de Prioridad').should('be.visible');
        });
    });

    describe('Formulario de Nuevo Ingreso', () => {
        beforeEach(() => {
            // Cambiar a la pestaña de Nuevo Ingreso
            cy.contains('Nuevo Ingreso').click();
        });

        it('Debe mostrar el formulario de registro', () => {
            cy.contains('Registrar Nuevo Paciente').should('be.visible');
            cy.contains('DNI del Paciente').should('be.visible');
            cy.contains('Informe Médico').should('be.visible');
            cy.contains('Nivel de Emergencia').should('be.visible');
            cy.contains('Signos Vitales').should('be.visible');
        });

        it('Debe validar campos obligatorios', () => {
            // Intentar enviar formulario vacío
            cy.contains('button', 'Registrar Paciente').click();

            // Verificar mensajes de error
            cy.contains('El CUIL del paciente es obligatorio').should('be.visible');
            cy.contains('El informe médico es obligatorio').should('be.visible');
        });

        it('Debe validar valores negativos en frecuencias', () => {
            // Llenar campos básicos
            cy.get('input[name="cuilPaciente"]').type('20-12345678-9');
            cy.get('textarea[name="informe"]').type('Paciente de prueba');

            // Ingresar frecuencia cardíaca negativa
            cy.get('input[name="frecuenciaCardiaca"]').type('-80');
            cy.get('input[name="frecuenciaRespiratoria"]').type('18');
            cy.get('input[name="frecuenciaSistolica"]').type('120');
            cy.get('input[name="frecuenciaDiastolica"]').type('80');

            // Intentar enviar
            cy.contains('button', 'Registrar Paciente').click();

            // Verificar mensaje de error
            cy.contains('no puede ser negativa').should('be.visible');
        });

        it('Debe permitir seleccionar diferentes niveles de emergencia', () => {
            cy.contains('Crítica').should('be.visible');
            cy.contains('Emergencia').should('be.visible');
            cy.contains('Urgencia').should('be.visible');
            cy.contains('Urgencia Menor').should('be.visible');
            cy.contains('Sin Urgencia').should('be.visible');

            // Seleccionar nivel crítico
            cy.contains('label', 'Crítica').click();
            cy.contains('label', 'Crítica').should('have.class', 'nivel-option-active');
        });
    });

    describe('Flujo Completo - Registrar Urgencia', () => {
        it('Debe registrar una nueva urgencia exitosamente', () => {
            // Ir al formulario
            cy.contains('Nuevo Ingreso').click();

            // Llenar el formulario
            cy.get('input[name="cuilPaciente"]').type('20-12345678-9');
            cy.get('textarea[name="informe"]').type('Paciente con dolor abdominal agudo. Requiere evaluación inmediata.');

            // Seleccionar nivel de emergencia
            cy.contains('label', 'Emergencia').click();

            // Llenar signos vitales
            cy.get('input[name="temperatura"]').type('38.5');
            cy.get('input[name="frecuenciaCardiaca"]').type('95');
            cy.get('input[name="frecuenciaRespiratoria"]').type('22');
            cy.get('input[name="frecuenciaSistolica"]').type('135');
            cy.get('input[name="frecuenciaDiastolica"]').type('88');

            // Interceptar la petición al backend
            cy.intercept('POST', `${apiUrl}/urgencias`).as('registrarUrgencia');

            // Enviar formulario
            cy.contains('button', 'Registrar Paciente').click();

            // Esperar respuesta del backend
            cy.wait('@registrarUrgencia').its('response.statusCode').should('eq', 201);

            // Verificar que muestra mensaje de éxito (alert)
            // Nota: Los alerts nativos no son ideales, considera usar un toast/notification
            cy.on('window:alert', (text) => {
                expect(text).to.contains('exitosamente');
            });

            // Verificar que vuelve a la cola de prioridad
            cy.contains('Cola de Prioridad', { timeout: 5000 }).should('be.visible');
        });
    });

    describe('Ordenamiento de Cola de Prioridad', () => {
        it('Debe ordenar pacientes por nivel de emergencia', () => {
            // Primero registramos varios pacientes con diferentes niveles

            // Paciente con urgencia menor
            cy.request({
                method: 'POST',
                url: `${apiUrl}/urgencias`,
                headers: {
                    'X-Enfermera-Matricula': matriculaEnfermera
                },
                body: {
                    cuilPaciente: '20-12345678-9',
                    informe: 'Caso menor',
                    temperatura: 36.5,
                    nivelEmergencia: 3, // Urgencia menor
                    frecuenciaCardiaca: 75,
                    frecuenciaRespiratoria: 16,
                    frecuenciaSistolica: 115,
                    frecuenciaDiastolica: 75
                }
            });

            // Paciente crítico
            cy.request({
                method: 'POST',
                url: `${apiUrl}/urgencias`,
                headers: {
                    'X-Enfermera-Matricula': matriculaEnfermera
                },
                body: {
                    cuilPaciente: '27-23456789-0',
                    informe: 'Caso crítico',
                    temperatura: 38.5,
                    nivelEmergencia: 0, // Crítico
                    frecuenciaCardiaca: 125,
                    frecuenciaRespiratoria: 28,
                    frecuenciaSistolica: 160,
                    frecuenciaDiastolica: 105
                }
            });

            // Recargar la página
            cy.reload();
            cy.contains('Cola de Prioridad').click();

            // Verificar que el paciente crítico está primero
            cy.get('.paciente-card').first().should('contain', 'Crítica');
        });
    });

    describe('Auto-actualización', () => {
        it('Debe actualizar la lista automáticamente cada 30 segundos', () => {
            cy.contains('Cola de Prioridad').click();

            // Interceptar peticiones de actualización
            cy.intercept('GET', `${apiUrl}/urgencias/lista-espera`).as('getListaEspera');

            // Esperar primera carga
            cy.wait('@getListaEspera');

            // Esperar 30 segundos y verificar que se hace otra petición
            // Nota: Esto haría la prueba muy lenta, mejor usar cy.clock()
            cy.clock();
            cy.tick(30000);
            cy.wait('@getListaEspera');

            cy.clock().invoke('restore');
        });
    });
});
