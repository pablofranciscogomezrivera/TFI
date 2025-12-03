# 🏥 Sistema de Gestión de Urgencias Clínicas
### Trabajo Final Integrador (TFI)

Proyecto desarrollado para la materia **Ingeniería de Software** del curso 4K2 en la **Universidad Tecnológica Nacional - Facultad Regional Tucumán**.

#### 👥 Integrantes del Grupo

| Legajo | Apellido y Nombre      |
| :--- |:---------------------|
| 47731  | Marcial, Gabriel       |
| 46646  | Ponce, Facundo         |
| 46380  | Cancino, Micaela       |
| 48235  | Bellor, Maria          |
| 52467  | Gómez, Pablo           |

---

## 📋 Descripción del Proyecto

El sistema está diseñado para gestionar el flujo crítico de admisiones en la sala de urgencias de una clínica. Su objetivo principal es optimizar el tiempo de atención mediante un sistema de **Triage**.

El flujo de trabajo contempla:
1.  **Admisión (Enfermería):** Registro de pacientes y toma de signos vitales.
2.  **Triage:** Clasificación automática o manual del nivel de urgencia (Crítica, Emergencia, Urgencia, etc.).
3.  **Cola de Espera:** Priorización automática de pacientes basada en su gravedad y tiempo de llegada.
4.  **Atención (Médicos):** Los doctores reclaman pacientes de la cola, visualizan su historial de ingreso y registran el diagnóstico/tratamiento para dar el alta.

La lógica de negocio se basa en el enfoque de **Desarrollo Guiado por el Comportamiento (BDD)**.

## 🛠️ Tecnologías Utilizadas

El sistema es una solución Full Stack moderna, separada en Backend (API) y Frontend (Cliente).

| Área | Componente | Tecnología |
| :--- | :--- | :--- |
| **Backend** | Framework | `.NET 8 (ASP.NET Core Web API)` |
| | Lenguaje | `C#` |
| | Base de Datos | `SQL Server` (Implementación con ADO.NET) |
| | Seguridad | `JWT` (JSON Web Tokens) + `BCrypt` |
| | Documentación | `Scalar` (OpenAPI/Swagger moderno) |
| | Validación | `FluentValidation` |
| **Frontend** | Librería UI | `React 19` |
| | Build Tool | `Vite` |
| | Http Client | `Axios` |
| **Calidad** | Pruebas BDD | `Reqnroll (Gherkin)` |
| | Unit Testing | `xUnit` |
| | Aserciones | `Fluent Assertions` |
| | CI/CD | `GitHub Actions` |

## 🏗️ Estructura del Proyecto

El sistema sigue una **Arquitectura en Capas (Clean Architecture)** para garantizar la separación de responsabilidades:

* **`Dominio`**: 🧠 Contiene las entidades (`Paciente`, `Ingreso`, `Usuario`), objetos de valor (`SignosVitales`) y reglas de negocio puras. No tiene dependencias externas.
* **`Aplicacion`**: ⚙️ Contiene los servicios (`ServicioUrgencias`, `ServicioAtencion`) y define las interfaces. Orquesta la lógica de negocio.
* **`Infraestructura`**: 💾 Implementa el acceso a datos. Utiliza **ADO.NET** (`RepositorioUrgenciasADO`, etc.) para comunicarse con SQL Server y ejecutar consultas optimizadas.
* **`Web` (API)**: 🌐 La capa de entrada. Contiene los Controllers, configuración de JWT, Inyección de Dependencias y la documentación con Scalar.
* **`cliente`**: 💻 Proyecto Frontend en React. Contiene las vistas para Enfermería (Ingresos) y Médicos (Atención), componentes UI y lógica de consumo de API.
* **`Tests`**: 🧪 Contiene todas las pruebas BDD (`.feature`) y Unitarias.

## 🚀 Guía de Instalación y Ejecución

### Prerrequisitos
* .NET 8.0 SDK
* Node.js (v18 o superior)
* SQL Server (LocalDB o instancia completa)

### 1. Configuración de Base de Datos
Asegúrate de que la cadena de conexión en `Web/appsettings.json` apunte a tu instancia local de SQL Server. El sistema incluye un **DataSeeder** que poblará datos iniciales (Usuarios, Obras Sociales) al arrancar.

### 2. Ejecutar el Backend (API)

```bash
# Navegar a la carpeta del proyecto Web
cd Web

# Restaurar dependencias
dotnet restore

# Ejecutar la aplicación
dotnet run
````

  * La API estará disponible en: `https://localhost:5284`
  * Documentación interactiva (Scalar): `https://localhost:5284/scalar`

### 3. Ejecutar el Frontend (Cliente)

```bash
# Navegar a la carpeta cliente
cd cliente

# Instalar dependencias
npm install

# Ejecutar servidor de desarrollo
npm run dev
```

  * La aplicación abrirá en: `http://localhost:5173`

-----

## 🧪 Cómo Ejecutar las Pruebas

Para verificar el comportamiento implementado, puedes ejecutar las pruebas automatizadas (Unitarias y BDD).

### Desde la Línea de Comandos

```bash
# Ubicarse en la raíz de la solución
# Ejecutar todos los tests
dotnet test TFI.sln
```

### En Visual Studio

1.  Abrir el **Explorador de Pruebas** (`Test > Explorador de pruebas`).
2.  Hacer clic en el botón "Ejecutar todas las pruebas" (ícono de play verde).

## 🔄 Funcionalidades Implementadas

### Módulo de Autenticación

  * Login con JWT para roles **Médico** y **Enfermera**.
  * Registro de nuevos usuarios con contraseñas hasheadas.

### Módulo de Pacientes

  * Búsqueda de pacientes por CUIL.
  * Registro de pacientes nuevos con validación de CUIL y Obras Sociales.

### Módulo de Urgencias (Enfermería)

  * Formulario de Triage.
  * Registro de signos vitales (Temperatura, FC, FR, Tensión).
  * Algoritmo de priorización (Niveles: Crítica, Emergencia, Urgencia, etc.).

### Módulo de Atención (Médicos)

  * Dashboard en tiempo real de la cola de espera.
  * Acción de "Llamar paciente" (cambia estado a `En Proceso`).
  * Registro de informe médico y finalización de la atención.

## 🤖 CI/CD - GitHub Actions

El proyecto cuenta con integración continua configurada.

  * ✅ **Backend Tests:** Se ejecuta automáticamente en cada Pull Request a la rama `main` o `master` para asegurar que no se rompa la lógica de negocio existente.

