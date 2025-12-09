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
        // Convert PascalCase to camelCase (e.g., 'CuilPaciente' -> 'cuilPaciente')
        const camelCaseKey = key.charAt(0).toLowerCase() + key.slice(1);

        // Backend usually returns an array of strings, we take the first one or join them
        const message = Array.isArray(backendErrors[key])
            ? backendErrors[key][0]
            : backendErrors[key];

        mappedErrors[camelCaseKey] = message;
    });

    return mappedErrors;
};
