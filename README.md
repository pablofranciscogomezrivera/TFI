# Sistema de Gestión de Urgencias Clínicas

## Trabajo Final Integrador (TFI)

Proyecto desarrollado para la materia **Ingeniería de Software** comisión 4K2 en la **Universidad Tecnológica Nacional - Facultad Regional Tucumán**.

### Integrantes del Grupo

| Legajo | Apellido y Nombre |
|:-------|:------------------|
| 47731  | Marcial, Gabriel  |
| 46646  | Ponce, Facundo    |
| 46380  | Cancino, Micaela  |
| 48235  | Bellor, Maria     |
| 52467  | Gómez, Pablo      |

---

## Descripción del Proyecto

El sistema está diseñado para gestionar el flujo crítico de admisiones en la sala de urgencias de una clínica. Su objetivo principal es optimizar el tiempo de atención mediante un sistema de **Triage**.

### Flujo de Trabajo

1. **Admisión (Enfermería):** Registro de pacientes y toma de signos vitales.
2. **Triage:** Clasificación automática o manual del nivel de urgencia (Crítica, Emergencia, Urgencia, Menor, Sin Urgencia).
3. **Cola de Espera:** Priorización automática de pacientes basada en su gravedad y tiempo de llegada.
4. **Atención (Médicos):** Los doctores reclaman pacientes de la cola, visualizan su historial de ingreso y registran el diagnóstico/tratamiento para dar el alta.

La lógica de negocio se basa en el enfoque de **Desarrollo Guiado por el Comportamiento (BDD)** utilizando Reqnroll (anteriormente SpecFlow).

---

## Tecnologías Utilizadas

El sistema es una solución Full Stack moderna, separada en Backend (API REST) y Frontend (SPA).

### Backend

| Componente          | Tecnología                    |
|:--------------------|:------------------------------|
| Framework           | .NET 8 (ASP.NET Core Web API) |
| Lenguaje            | C# 12                         |
| Base de Datos       | SQL Server                    |
| Acceso a Datos      | ADO.NET                       |
| Seguridad           | JWT + BCrypt                  |
| Mapeo de Objetos    | AutoMapper                    |
| Documentación API   | Swagger / Scalar              |
| Validación          | FluentValidation              |

### Frontend

| Componente          | Tecnología                    |
|:--------------------|:------------------------------|
| Librería UI         | React 19                      |
| Build Tool          | Vite 7                        |
| HTTP Client         | Axios                         |
| Layout System       | Bootstrap Grid 5              |
| Estilos             | CSS Custom + Bootstrap Grid   |
| Routing             | React Router DOM 7            |

### Testing & CI/CD

| Componente          | Tecnología                    |
|:--------------------|:------------------------------|
| Pruebas BDD         | Reqnroll (Gherkin)            |
| Unit Testing        | xUnit                         |
| Mocking             | NSubstitute                   |
| Aserciones          | Fluent Assertions             |
| CI/CD               | GitHub Actions                |

---

## Arquitectura del Proyecto

El sistema sigue una **Arquitectura en Capas (Clean Architecture)** para garantizar la separación de responsabilidades y facilitar el mantenimiento.

### Estructura de Directorios

```
TFI/
├── Dominio/              # Entidades, Value Objects, Interfaces
│   ├── Entidades/        # Paciente, Ingreso, Usuario, Atencion
│   ├── Enums/            # NivelEmergencia, EstadoIngreso
│   ├── Interfaces/       # IRepositorioPacientes, IRepositorioUrgencias
│   └── Validadores/      # ValidadorCUIL, ValidadorEmail
├── Aplicacion/           # Servicios de aplicación
│   ├── Servicios/        # ServicioUrgencias, ServicioAtencion, ServicioPacientes
│   ├── Interfaces/       # IServicioUrgencias, IServicioAtencion, IPasswordHasher
│   └── DTOs/             # NuevaUrgenciaDto, DatosNuevoPacienteDto
├── Infraestructura/      # Acceso a datos
│   ├── Repositorios/     # RepositorioPacientesADO, RepositorioUrgenciasADO
│   └── Servicios/        # BCryptPasswordHasher
├── Web/                  # API REST
│   ├── Controllers/      # UrgenciasController, PacientesController, AuthController
│   ├── DTOs/             # Request/Response objects
│   ├── Validators/       # FluentValidation validators
│   ├── Mapping/          # MappingProfile (AutoMapper)
│   ├── Middleware/       # GlobalExceptionHandler
│   └── Helpers/          # CuilHelper, JWT configuration
├── cliente/              # Frontend React
│   ├── src/
│   │   ├── components/   # Componentes reutilizables
│   │   ├── pages/        # LoginPage, RegisterPage, UrgenciasPage
│   │   ├── services/     # API clients (axios)
│   │   ├── utils/        # Helpers (cuilHelper, jwtHelper)
│   │   └── constants/    # Enums, configuración
└── Tests/                # Pruebas automatizadas
    ├── Features/         # Archivos .feature (Gherkin)
    ├── StepDefinitions/  # Implementación de steps BDD
    └── UnitTests/        # Tests unitarios por capa
```

### Descripción de Capas

#### Dominio
Contiene las entidades del negocio, objetos de valor y reglas de negocio puras. No tiene dependencias externas.

**Entidades principales:**
- `Paciente`: Información del paciente (CUIL, nombre, obra social)
- `Ingreso`: Registro de urgencia con signos vitales y nivel de emergencia
- `Usuario`: Credenciales y rol (Médico/Enfermera)
- `Atencion`: Registro médico del diagnóstico y tratamiento

**Enums:**
- `NivelEmergencia`: CRITICA, EMERGENCIA, URGENCIA, URGENCIA_MENOR, SIN_URGENCIA
- `EstadoIngreso`: PENDIENTE, EN_PROCESO, FINALIZADO

**Validadores:**
- `ValidadorCUIL`: Validación completa con dígito verificador
- `ValidadorEmail`: Validación de formato de email

#### Aplicación
Orquesta la lógica de negocio y define los casos de uso del sistema. Utiliza el **Parameter Object Pattern** para métodos con múltiples parámetros.

**Servicios principales:**
- `ServicioUrgencias`: Gestión de ingresos y cola de prioridad
- `ServicioAtencion`: Registro de atención médica
- `ServicioPacientes`: Búsqueda y registro de pacientes
- `ServicioAutenticacion`: Login y registro de usuarios

**DTOs de Aplicación:**
- `NuevaUrgenciaDto`: Agrupa datos de urgencia + datos opcionales del paciente
- `DatosNuevoPacienteDto`: Datos anidados del paciente (nombre, domicilio, obra social)

**Interfaces:**
- `IPasswordHasher`: Abstracción para operaciones de hashing (implementado por BCryptPasswordHasher)

#### Infraestructura
Implementa el acceso a datos y servicios externos.

**Repositorios (ADO.NET):**
- Queries parametrizadas para prevenir SQL Injection
- Transacciones para operaciones complejas
- Connection pooling optimizado
- Manejo robusto de excepciones

**Servicios:**
- `BCryptPasswordHasher`: Implementación de IPasswordHasher usando BCrypt

#### Web (API)
Capa de entrada HTTP que expone los endpoints REST.

**Características:**
- Autenticación JWT con claims personalizados
- Autorización por roles (Médico/Enfermera)
- Validación automática con FluentValidation
- Mapeo automático con AutoMapper (`MappingProfile`)
- Manejo global de excepciones (`GlobalExceptionHandler`)
- Documentación interactiva con Swagger/Scalar
- CORS configurado para desarrollo

**Middleware:**
- `GlobalExceptionHandler`: Centraliza el manejo de errores y mapea excepciones a códigos HTTP apropiados

#### Cliente (Frontend)
Aplicación React SPA con diseño responsive.

**Características:**
- Bootstrap Grid System para layouts responsive
- Componentes UI reutilizables
- Gestión de estado con React Hooks
- Validación centralizada (cuilHelper)
- JWT almacenado en localStorage
- Notificaciones visuales

---

## Patrones de Diseño Implementados

### Parameter Object Pattern
Utilizado en `ServicioUrgencias.RegistrarUrgencia()` para agrupar 15+ parámetros en un objeto DTO:

```csharp
// Antes (Code Smell: Long Parameter List)
void RegistrarUrgencia(string cuil, Enfermera enfermera, string informe, 
    double temperatura, NivelEmergencia nivel, double frecCardiaca, 
    double frecRespiratoria, double sistolica, double diastolica, 
    string? nombrePaciente, string? apellidoPaciente, ...);

// Después (Parameter Object Pattern)
void RegistrarUrgencia(NuevaUrgenciaDto datos, Enfermera enfermera);
```

### Dependency Inversion
- `IPasswordHasher` (Aplicación) → `BCryptPasswordHasher` (Infraestructura)
- Interfaces de repositorios en Dominio, implementaciones en Infraestructura

### Exception Handler Pattern
`GlobalExceptionHandler` centraliza el manejo de excepciones:
- `ArgumentException` → 400 Bad Request
- `InvalidOperationException` → 409 Conflict
- `UnauthorizedAccessException` → 401 Unauthorized
- Otras → 500 Internal Server Error

---

## Instalación y Configuración

### Prerrequisitos

- **.NET 8.0 SDK** o superior
- **Node.js v18** o superior
- **SQL Server** (LocalDB, Express o Standard)
- **Git** para clonar el repositorio
- **Visual Studio 2022** (Community, Professional o Enterprise)

### 1. Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/TFI.git
cd TFI
```

### 1.1 Abrir el Proyecto en Visual Studio

#### Opción A: Abrir la Solución

1. Abre Visual Studio 2022
2. Click en "Abrir un proyecto o una solución"
3. Navega a la carpeta del proyecto
4. Selecciona `TFI.sln`

#### Opción B: Desde el Explorador de Archivos

1. Navega a la carpeta del proyecto
2. Doble click en `TFI.sln`
3. Se abrirá automáticamente en Visual Studio

### 1.2 Configurar Visual Studio

#### Workloads Necesarias

Asegúrate de tener instaladas las siguientes cargas de trabajo:

1. Abre Visual Studio Installer
2. Click en "Modificar" en tu instalación de Visual Studio 2022
3. Selecciona:
   - **Desarrollo de ASP.NET y web** (para el backend)
   - **Desarrollo de escritorio de .NET** (para ejecutar tests)
   - **Almacenamiento y procesamiento de datos** (para trabajar con SQL Server)

### 2. Configuración de Base de Datos

#### Opción A: SQL Server LocalDB (Recomendado para desarrollo)

La cadena de conexión por defecto en `Web/appsettings.json` usa LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UrgenciasDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

#### Opción B: SQL Server completo

Si usas una instancia de SQL Server, modifica la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=UrgenciasDB;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True;"
}
```

#### Inicialización de Datos

Para inicializar la base de datos:

1. Ejecuta el script SQL `QueryInicial.sql` ubicado en la raíz del proyecto:

```bash
sqlcmd -S localhost -i QueryInicial.sql
```

O desde SQL Server Management Studio:
- Abre `QueryInicial.sql`
- Ejecuta el script

Este script crea:
- Base de datos `HospitalUrgencias`
- Todas las tablas necesarias (Pacientes, Ingresos, Usuarios, Doctores, Enfermeros, ObrasSociales, etc.)
- Obras sociales iniciales (OSDE, Swiss Medical, Galeno, etc.)
- Padrón de afiliados de prueba

### 3. Ejecutar el Backend

#### Desde Visual Studio (Recomendado)

1. En el **Explorador de soluciones**, click derecho en el proyecto **Web**
2. Selecciona "Establecer como proyecto de inicio"
3. Presiona `F5` o click en el botón de play verde
4. Se abrirá automáticamente el navegador con la documentación de Scalar

La API estará disponible en:
- **HTTPS:** `https://localhost:5284`
- **HTTP:** `http://localhost:5285`
- **Documentación:** `https://localhost:5284/scalar`

### 4. Ejecutar el Frontend

```bash
cd cliente
npm install
npm run dev
```

La aplicación estará disponible en: `http://localhost:5173`

---

## Ejecución de Pruebas

### Desde Visual Studio

1. Abre el **Explorador de pruebas:** `Ctrl+E, T`
2. Click en "Ejecutar todas las pruebas"

### Desde Terminal

```bash
# Todas las pruebas
dotnet test TFI.sln

# Solo pruebas de Urgencias
dotnet test --filter "FullyQualifiedName~Urgencias"
```

---

## Funcionalidades Implementadas

### Módulo de Autenticación

- Login con JWT para roles Médico y Enfermera
- Registro de nuevos usuarios con validación
- Contraseñas hasheadas con BCrypt (via `IPasswordHasher`)
- Tokens con expiración configurable
- Claims personalizados (matrícula, rol)

**Endpoints:**
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/registrar` - Registrar nuevo usuario

### Módulo de Pacientes

- Búsqueda de pacientes por CUIL
- Registro de pacientes nuevos
- Validación de CUIL con dígito verificador
- Asociación con obras sociales

**Endpoints:**
- `GET /api/pacientes/{cuil}` - Buscar paciente
- `POST /api/pacientes` - Registrar paciente
- `GET /api/obrassociales` - Listar obras sociales

### Módulo de Urgencias (Enfermería)

- Formulario de triage responsive
- Registro de signos vitales (Temperatura, FC, FR, TA)
- Clasificación de emergencia con colores
- Cola de prioridad automática
- Verificación automática de pacientes existentes

**Endpoints:**
- `POST /api/urgencias` - Registrar nuevo ingreso
- `GET /api/urgencias/lista-espera` - Obtener cola de espera

### Módulo de Atención (Médicos)

- Dashboard en tiempo real
- Acción "Llamar Siguiente Paciente"
- Modal con resumen del ingreso
- Registro de diagnóstico médico
- Finalización y alta del paciente
- Cancelación de atención (devuelve a cola)

**Endpoints:**
- `POST /api/urgencias/reclamar` - Reclamar paciente
- `POST /api/atenciones` - Registrar atención
- `POST /api/urgencias/cancelar/{cuil}` - Cancelar atención

---

## CI/CD - GitHub Actions

El proyecto incluye integración continua automática.

**Backend Tests:** `.github/workflows/backend-tests.yml`
- Se ejecuta en cada push y pull request
- Ejecuta todas las pruebas (BDD + Unit)
- Verifica compilación exitosa

---

## Estructura de Base de Datos

### Tablas Principales

**Pacientes:** CUIL (PK), Nombre, Apellido, FechaNacimiento, Dirección, Email, Teléfono, ObraSocialId

**Usuarios:** Email (PK), PasswordHash, Nombre, Apellido, Matrícula, TipoAutoridad (0=Médico, 1=Enfermera)

**Ingresos:** ID (PK), CUILPaciente (FK), DNIEnfermera (FK), FechaIngreso, NivelEmergencia, Estado, SignosVitales, InformeEnfermera

**Atenciones:** ID (PK), IngresoId (FK), CUILPaciente (FK), MatrículaMédico (FK), FechaAtención, InformeMédico

**ObrasSociales:** ID (PK), Nombre, Descripción
