//// ***********************************************
//// This example commands.js shows you how to
//// create various custom commands and overwrite
//// existing commands.
//// ***********************************************

//// Comando para login (cuando se implemente autenticación)
//Cypress.Commands.add('login', (email, password) => {
//    const apiUrl = Cypress.env('apiUrl');
//    cy.request({
//        method: 'POST',
//        url: `${apiUrl}/auth/login`,
//        body: {
//            email,
//            password
//        }
//    }).then((response) => {
//        // Guardar token en localStorage si se usa JWT
//        if (response.body.token) {
//            window.localStorage.setItem('authToken', response.body.token);
//        }
//    });
//});

//// Comando para cerrar sesión
//Cypress.Commands.add('logout', () => {
//    window.localStorage.removeItem('authToken');
//});