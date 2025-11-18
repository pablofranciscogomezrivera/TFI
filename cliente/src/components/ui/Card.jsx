import React from 'react'
import './Card.css';

export const Card = ({ children, className = '', onClick }) => {
    return (
        <div
            className={`card ${className} ${onClick ? 'card-clickable' : ''}`}
            onClick={onClick}
        >
            {children}
        </div>
    );
};

export default Card;