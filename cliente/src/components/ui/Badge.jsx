import React from 'react'
import './Badge.css';

export const Badge = ({ children, variant = 'default' }) => {
    return (
        <span className={`badge badge-${variant}`}>
            {children}
        </span>
    );
};

export default Badge;