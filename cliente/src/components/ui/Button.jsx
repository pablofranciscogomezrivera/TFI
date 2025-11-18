import React from 'react'
import './Button.css';

export const Button = ({
    children,
    onClick,
    variant = 'primary',
    size = 'medium',
    fullWidth = false,
    disabled = false,
    icon = null,
    type = 'button'
}) => {
    return (
        <button
            className={`btn btn-${variant} btn-${size} ${fullWidth ? 'btn-full-width' : ''}`}
            onClick={onClick}
            disabled={disabled}
            type={type}
        >
            {icon && <span className="btn-icon">{icon}</span>}
            {children}
        </button>
    );
};

export default Button;