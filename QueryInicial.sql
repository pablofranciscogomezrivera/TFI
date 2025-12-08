CREATE DATABASE HospitalUrgencias;
GO
USE HospitalUrgencias;
GO

-- Tabla Obras Sociales
CREATE TABLE ObrasSociales (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL
);

-- Tabla Domicilios
CREATE TABLE Domicilios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Calle NVARCHAR(100) NOT NULL,
    Numero INT NOT NULL,
    Localidad NVARCHAR(100) NOT NULL,
    Ciudad NVARCHAR(100) NOT NULL,
    Provincia NVARCHAR(100) NOT NULL
);

-- Tabla Pacientes
CREATE TABLE Pacientes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CUIL NVARCHAR(13) NOT NULL UNIQUE, 
    DNI INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150),
    Telefono BIGINT,
    FechaNacimiento DATETIME2 NOT NULL,
    IdDomicilio INT NOT NULL,
    IdObraSocial INT NULL, -- Puede ser nulo si no tiene
    NumeroAfiliado NVARCHAR(50) NULL,
    CONSTRAINT FK_Pacientes_Domicilios FOREIGN KEY (IdDomicilio) REFERENCES Domicilios(Id),
    CONSTRAINT FK_Pacientes_ObrasSociales FOREIGN KEY (IdObraSocial) REFERENCES ObrasSociales(Id)
);

INSERT INTO ObrasSociales (Nombre) VALUES 
('OSDE'), ('Swiss Medical'), ('Subsidio de Salud'), ('Fondo de Bikini SA'), ('Galeno');


-- 1. Tabla para simular el Padrón de afiliados válidos
CREATE TABLE PadronAfiliados (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdObraSocial INT NOT NULL,
    NumeroAfiliado NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(100),
    Apellido NVARCHAR(100),
    CONSTRAINT FK_Padron_ObraSocial FOREIGN KEY (IdObraSocial) REFERENCES ObrasSociales(Id)
);

-- 2. Insertar afiliados autorizados ("Lista Blanca")
INSERT INTO PadronAfiliados (IdObraSocial, NumeroAfiliado, Nombre, Apellido)
VALUES (1, 'PG999', 'Pedro', 'González');

-- Insertar otros afiliados de prueba si quieres
INSERT INTO PadronAfiliados (IdObraSocial, NumeroAfiliado) VALUES (2, '67890'); -- Swiss Medical
INSERT INTO PadronAfiliados (IdObraSocial, NumeroAfiliado) VALUES (3, 'ABC123'); -- Subsidio



CREATE TABLE Ingresos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdPaciente INT NOT NULL,
    
    -- Guardamos las matrículas para saber quién hizo qué
    MatriculaEnfermera NVARCHAR(50) NOT NULL,
    MatriculaDoctor NVARCHAR(50) NULL,
    
    FechaIngreso DATETIME2 NOT NULL,
    Informe NVARCHAR(MAX) NOT NULL,
    InformeMedico NVARCHAR(MAX) NULL, 
    
    -- Enums guardados como números (0, 1, 2...)
    NivelEmergencia INT NOT NULL, 
    Estado INT NOT NULL,          
    
    -- Signos Vitales
    Temperatura FLOAT NOT NULL,
    FrecuenciaCardiaca FLOAT NOT NULL,
    FrecuenciaRespiratoria FLOAT NOT NULL,
    TensionSistolica FLOAT NOT NULL,
    TensionDiastolica FLOAT NOT NULL,

    CONSTRAINT FK_Ingresos_Pacientes FOREIGN KEY (IdPaciente) REFERENCES Pacientes(Id)
);


CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    TipoAutoridad INT NOT NULL -- 0: Medico, 1: Enfermera
);


-- Tabla para Enfermeras vinculada a un Usuario
CREATE TABLE Enfermeros (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI INT NOT NULL,
    CUIL NVARCHAR(13) NOT NULL,
    Matricula NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NULL,
    Telefono BIGINT NULL,
    FechaNacimiento DATETIME2 NULL,
    CONSTRAINT FK_Enfermeras_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id)
);

-- Tabla para Doctores vinculada a un Usuario
CREATE TABLE Doctores (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdUsuario INT NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI INT NOT NULL,
    CUIL NVARCHAR(13) NOT NULL,
    Matricula NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NULL,
    Telefono BIGINT NULL,
    FechaNacimiento DATETIME2 NULL,
    CONSTRAINT FK_Doctores_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id)
);
