/**
 * Maps known backend field names to frontend field names.
 * Handles various casing scenarios (CUIL, cuil, Cuil, cUIL -> cuil).
 */
const fieldMappings = {
    // CUIL variations
    'cuil': 'cuil',
    'CUIL': 'cuil',
    'Cuil': 'cuil',
    'cUIL': 'cuil',
    // DNI variations  
    'dni': 'dni',
    'DNI': 'dni',
    'Dni': 'dni',
    // Other common fields
    'email': 'email',
    'Email': 'email',
    'password': 'password',
    'Password': 'password',
    'nombre': 'nombre',
    'Nombre': 'nombre',
    'apellido': 'apellido',
    'Apellido': 'apellido',
    'matricula': 'matricula',
    'Matricula': 'matricula',
};

/**
 * Converts a key to camelCase.
 * First checks known mappings, then applies generic conversion.
 * @param {string} key - The key to convert.
 * @returns {string} - The camelCase version of the key.
 */
const toCamelCase = (key) => {
    // Check if we have a known mapping
    if (fieldMappings[key]) {
        return fieldMappings[key];
    }

    // If the key is all uppercase (e.g., 'CUIL', 'DNI'), convert to lowercase
    if (key === key.toUpperCase() && key.length > 1) {
        return key.toLowerCase();
    }
    // Otherwise, just lowercase the first character (PascalCase -> camelCase)
    return key.charAt(0).toLowerCase() + key.slice(1);
};

/**
 * Maps backend validation errors (PascalCase) to frontend specific keys (camelCase).
 * @param {Object} backendErrors - The errors object from the backend (e.g. response.data.errors).
 * @returns {Object} - A new object with camelCase keys and error messages.
 */
export const mapValidationErrors = (backendErrors) => {
    if (!backendErrors || typeof backendErrors !== 'object') {
        return {};
    }

    const mappedErrors = {};

    Object.keys(backendErrors).forEach((key) => {
        // Convert PascalCase/UPPERCASE to camelCase (e.g., 'CuilPaciente' -> 'cuilPaciente', 'CUIL' -> 'cuil')
        const camelCaseKey = toCamelCase(key);

        // Backend usually returns an array of strings, we take the first one or join them
        const message = Array.isArray(backendErrors[key])
            ? backendErrors[key][0]
            : backendErrors[key];

        mappedErrors[camelCaseKey] = message;
    });

    return mappedErrors;
};
