import React from 'react'
import './Input.css';

export const Input = ({
    label,
    value,
    onChange,
    type = 'text',
    placeholder = '',
    required = false,
    error = '',
    disabled = false,
    readOnly = false,
    name = '',
    style = {}
}) => {
    return (
        <div className="input-group">
            {label && (
                <label className="input-label">
                    {label}
                    {required && <span className="input-required">*</span>}
                </label>
            )}
            <input
                type={type}
                className={`input-field ${error ? 'input-error' : ''}`}
                value={value}
                onChange={onChange}
                placeholder={placeholder}
                disabled={disabled}
                readOnly={readOnly}
                name={name}
                required={required}
                style={style}
            />
            {error && <span className="input-error-message">{error}</span>}
        </div>
    );
};

export const TextArea = ({
    label,
    value,
    onChange,
    placeholder = '',
    required = false,
    error = '',
    disabled = false,
    name = '',
    rows = 4
}) => {
    return (
        <div className="input-group">
            {label && (
                <label className="input-label">
                    {label}
                    {required && <span className="input-required">*</span>}
                </label>
            )}
            <textarea
                className={`input-field input-textarea ${error ? 'input-error' : ''}`}
                value={value}
                onChange={onChange}
                placeholder={placeholder}
                disabled={disabled}
                name={name}
                required={required}
                rows={rows}
            />
            {error && <span className="input-error-message">{error}</span>}
        </div>
    );
};

export default Input;