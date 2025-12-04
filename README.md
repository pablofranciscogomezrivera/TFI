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

## Capturas de pantalla

 * Flujo de enfermera:
 1- Inicio de sesión
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/7ff3312e-52b8-43d4-8425-604ee75b92c8" />
 ✅ Popup de inicio exitoso:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/e575f58a-4921-4709-aecc-dd9082d9eaa0" />

 2- Panel de cola de prioridad e ingreso de pacientes:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/c93bb8dc-f9d4-4cc3-8af8-a455bb43e1b3" />
 2.1- Nuevo ingreso:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/aa6dfdf2-2ea8-4a57-9896-1ac387295234" />
 2.1.1- Ejemplo de carga paciente nuevo:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/25f9424b-f156-4589-b770-8ad4b4008b24" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/b20f0b3b-465f-4dc5-a95b-023b1f7d5376" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/40fcf105-cd4a-4142-a757-5dfecc6f8531" />
 2.1.2- Reconoce paciente nuevo:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/114aa451-02dd-4da5-be99-fa8070c1f183" />
 2.1.2.1- Carga de paciente nuevo:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/d910e10a-88cf-4e1e-b50e-589942ddd6ee" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/7cf1c880-7b96-4bcc-bcb5-7682b78dd19c" />
 2.2- Ingreso exitoso en cola de prioridad con popup de exito:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/c939f9d5-a704-4b54-ab81-876d6e7d2a19" />

 3- Ingreso de paciente existente y con prioridad mayor al anterior:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/244b4eee-2395-4c7e-87f2-987ec82d414a" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/31e42ff6-0c85-4d2e-bd37-1287c14163fb" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/d1538ad6-00c8-4f0a-8b9d-6da577f50831" />
 3.1- Ingreso exitoso y acomodado en cola de prioridad:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/5657c99f-5995-459c-9b55-7b5b4ba39360" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/dad96b5d-ab0e-40d6-97d1-c7bf7fc23967" />

 *Flujo medico:
 1- Inicio de sesión
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/560d865d-39d6-41d0-8b17-ff9c3ba902c5" />
 ✅ Popup de inicio exitoso:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/fcd6c3c4-33f0-4930-a20c-232a7effce16" />

 2- Panel de medico de la cola de prioridad con posibilidad de atender al paciente para generar su informe:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/8b6dcf3a-0058-4eef-bfe1-f8948c6306fd" />
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/6d175fa3-79c5-4ce5-974f-4b3dbd9929c8" />

 3- Llamar al paciente para ser atendido por el medico:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/0b5b12c2-80bf-4e46-80ba-4d349f86e44e" />
 3.1- Panel de registro de informe con resumen del diagnostico preventivo del ingreso:
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/61c88c12-e406-470a-8c16-02a61a339a9a" />
 3.2- Confirmacion del registro y dado de alta del paciente con popup de exito (con su eliminacion de la cola de prioridad):
 <img width="1919" height="1079" alt="image" src="https://github.com/user-attachments/assets/a9828d18-809c-43fd-8a42-e7d8534a8580" />
 <img width="1919" height="1029" alt="image" src="https://github.com/user-attachments/assets/fe0ee962-9a49-47b1-8853-d086cc82594f" />























