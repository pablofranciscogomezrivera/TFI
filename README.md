# Sistema de Gestión de Urgencias Clínicas

## Trabajo Final Integrador (TFI)

Proyecto desarrollado para la materia **Ingeniería de Software** del curso 4K2 en la **Universidad Tecnológica Nacional - Facultad Regional Tucumán**.

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
| Documentación API   | Swagger                       |
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
│   ├── Interfaces/       # IRepositorioPacientes, IRepositorioUrgencias
│   └── Validadores/      # ValidadorCUIL, ValidadorEmail
├── Aplicacion/           # Servicios de aplicación
│   ├── Servicios/        # ServicioUrgencias, ServicioAtencion, ServicioPacientes
│   └── Interfaces/       # IServicioUrgencias, IServicioAtencion
├── Infraestructura/      # Acceso a datos
│   └── Repositorios/     # RepositorioPacientesADO, RepositorioUrgenciasADO
├── Web/                  # API REST
│   ├── Controllers/      # UrgenciasController, PacientesController, AuthController
│   ├── DTOs/             # Request/Response objects
│   ├── Validators/       # FluentValidation validators
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

**Validadores:**
- `ValidadorCUIL`: Validación completa con dígito verificador
- `ValidadorEmail`: Validación de formato de email

#### Aplicación
Orquesta la lógica de negocio y define los casos de uso del sistema.

**Servicios principales:**
- `ServicioUrgencias`: Gestión de ingresos y cola de prioridad
- `ServicioAtencion`: Registro de atención médica
- `ServicioPacientes`: Búsqueda y registro de pacientes
- `ServicioAutenticacion`: Login y registro de usuarios

#### Infraestructura
Implementa el acceso a datos utilizando ADO.NET para comunicarse con SQL Server.

**Características:**
- Queries parametrizadas para prevenir SQL Injection
- Transacciones para operaciones complejas
- Connection pooling optimizado
- Manejo robusto de excepciones

#### Web (API)
Capa de entrada HTTP que expone los endpoints REST.

**Características:**
- Autenticación JWT
- Autorización por roles
- Validación con FluentValidation
- Documentación interactiva con Scalar
- CORS configurado para desarrollo

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

#### Opción C: Clonar desde Visual Studio

1. Abre Visual Studio 2022
2. Click en "Clonar un repositorio"
3. Ingresa la URL: `https://github.com/tu-usuario/TFI.git`
4. Selecciona la ubicación local
5. Visual Studio clonará y abrirá la solución automáticamente

### 1.2 Configurar Visual Studio

#### Workloads Necesarias

Asegúrate de tener instaladas las siguientes cargas de trabajo:

1. Abre Visual Studio Installer
2. Click en "Modificar" en tu instalación de Visual Studio 2022
3. Selecciona:
   - **Desarrollo de ASP.NET y web** (para el backend)
   - **Desarrollo de escritorio de .NET** (para ejecutar tests)
   - **Almacenamiento y procesamiento de datos** (para trabajar con SQL Server)

#### Configuración Recomendada

**Herramientas > Opciones:**

- **Entorno > General:**
  - Tema: Azul/Oscuro según preferencia
  
- **Editor de texto > C# > Avanzado:**
  - Habilitar "Sugerencias de IntelliSense"
  - Habilitar "Navegar a código descompilado"

- **Depurador > General:**
  - Habilitar "Usar solo mi código"
  - Habilitar "Habilitar editar y continuar"

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

#### Opción A: Usando Visual Studio (Recomendado)

1. En el **Explorador de soluciones**, click derecho en el proyecto **Web**
2. Selecciona "Establecer como proyecto de inicio" (aparecerá en negrita)
3. Presiona `F5` o click en el botón de play verde "Iniciar" en la barra superior
4. Visual Studio compilará y ejecutará la aplicación
5. Se abrirá automáticamente el navegador con la documentación de Scalar

**Proyecto de inicio correcto:**
```
TFI (Solución)
├── Dominio
├── Aplicacion
├── Infraestructura
├── Web  ← Este debe estar en negrita (proyecto de inicio)
├── cliente
└── Tests
```

#### Opción B: Desde la Consola del Administrador de Paquetes

1. Herramientas > Administrador de paquetes NuGet > Consola del Administrador de paquetes
2. Asegúrate de que el proyecto predeterminado sea "Web"
3. Ejecuta: `dotnet run`

La API estará disponible en:
- **HTTPS:** `https://localhost:5284`
- **HTTP:** `http://localhost:5285`
- **Documentación:** `https://localhost:5284/scalar`

#### Ver Logs en Visual Studio

Los logs aparecen en:
- **Ventana de salida:** Ver > Salida (Ctrl+Alt+O)
- Selecciona "Mostrar resultados desde: Depuración" en el desplegable

#### Detener la Aplicación

- Presiona `Shift+F5`
- O click en el botón rojo "Detener" en la barra superior

#### Crear Usuarios

La base de datos no incluye usuarios por defecto. Debes crear usuarios mediante:

1. **Endpoint de registro:** `POST /api/auth/registrar`
2. **SQL directo:** Insertando en las tablas `Usuarios` y `Doctores`/`Enfermeros`

**Nota:** Las contraseñas se almacenan hasheadas con BCrypt. Tambien puedes registar usuarios ejecutando el frontend en simultaneo.

### 4. Ejecutar el Frontend

El frontend es una aplicación React separada que debe ejecutarse en una terminal externa.

#### Opción A: Desde Visual Studio - Terminal del Desarrollador

1. En Visual Studio: Ver > Terminal
2. Navega a la carpeta cliente:
```bash
cd cliente
```

3. Instala dependencias (solo la primera vez):
```bash
npm install
```

4. Ejecuta el servidor de desarrollo:
```bash
npm run dev
```

#### Opción B: Desde Símbolo del sistema

1. Abre una nueva ventana de CMD o PowerShell
2. Navega a la carpeta del proyecto:
```bash
cd ruta\al\proyecto\TFI\cliente
```

3. Ejecuta:
```bash
npm install
npm run dev
```

La aplicación estará disponible en:
- **URL:** `http://localhost:5173`

#### Ejecutar Backend y Frontend Simultáneamente

**Flujo de trabajo recomendado:**

1. **En Visual Studio:** 
   - Presiona `F5` para iniciar el backend
   - La API correrá en `https://localhost:5284`

2. **En una terminal separada:**
   - Ejecuta `npm run dev` en la carpeta `cliente`
   - El frontend correrá en `http://localhost:5173`

3. **Probar la aplicación:**
   - Abre el navegador en `http://localhost:5173`
   - El frontend se comunicará automáticamente con el backend

#### Configuración de API

El frontend está configurado para conectarse a `http://localhost:5285` por defecto. Si tu API usa otro puerto, modifica `cliente/src/services/api.js`:

```javascript
const api = axios.create({
    baseURL: 'http://localhost:TU_PUERTO'
});
```

---

## Ejecución de Pruebas

### Pruebas Backend (BDD + Unit Tests)

#### Opción A: Usando el Explorador de Pruebas de Visual Studio

1. Abre el **Explorador de pruebas:**
   - Prueba > Explorador de pruebas
   - O presiona `Ctrl+E, T`

2. Visual Studio descubrirá automáticamente todas las pruebas

3. Verás la estructura:
```
Tests
├── Features
│   └── ModuloDeUrgenciasStepDefinitions (BDD Tests)
└── UnitTests
    ├── API
    ├── Aplicacion
    └── Dominio
```

4. **Ejecutar pruebas:**
   - **Ejecutar todas:** Click en el botón "Ejecutar todas las pruebas" (doble play)
   - **Ejecutar una:** Click derecho en la prueba > Ejecutar
   - **Debug una prueba:** Click derecho > Depurar

5. **Filtrar pruebas:**
   - Usa la barra de búsqueda para filtrar por nombre
   - Click en los íconos de estado (Passed/Failed) para filtrar

#### Opción B: Desde el Editor de Código

Visual Studio muestra indicadores junto a cada método de prueba:

```csharp
[Fact]                    // ← Verás un ícono verde/rojo aquí
public void DeberiaValidarCUIL()
{
    // Código de la prueba
}
```

Click en el ícono para:
- **Play verde:** Ejecutar la prueba
- **Bug:** Depurar la prueba

#### Opción C: Desde la Terminal

1. Ver > Terminal
2. Ejecuta:

```bash
# Todas las pruebas
dotnet test TFI.sln

# Solo pruebas de un proyecto
dotnet test Tests/Tests.csproj

# Con verbose para más detalles
dotnet test TFI.sln --verbosity normal
```

#### Ver Resultados

Los resultados aparecen en:
- **Explorador de pruebas:** Vista gráfica con ✓ (passed) y ✗ (failed)
- **Ventana de salida:** Detalles completos de ejecución
- **Lista de errores:** Pruebas fallidas con mensajes de error

### Cobertura de Código

#### Usando Fine Code Coverage (Extensión recomendada)

1. Instala la extensión "Fine Code Coverage" desde el Marketplace de Visual Studio

2. Ejecuta las pruebas desde el Explorador de pruebas

3. La cobertura se mostrará automáticamente:
   - En el editor verás líneas resaltadas:
     - **Verde:** Código cubierto
     - **Rojo:** Código no cubierto
   - Panel de cobertura con porcentajes por clase/método

#### Desde Terminal

```bash
# Generar reporte de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Con reporte HTML
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=html
```

---

## Funcionalidades Implementadas

### Módulo de Autenticación

**Características:**
- Login con JWT para roles Médico y Enfermera
- Registro de nuevos usuarios con validación
- Contraseñas hasheadas con BCrypt
- Tokens con expiración configurable
- Claims personalizados (matrícula, rol)

**Endpoints:**
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/registrar` - Registrar nuevo usuario

### Módulo de Pacientes

**Características:**
- Búsqueda de pacientes por CUIL
- Registro de pacientes nuevos
- Validación de CUIL con dígito verificador
- Asociación con obras sociales
- Historial de ingresos

**Endpoints:**
- `GET /api/pacientes/{cuil}` - Buscar paciente
- `POST /api/pacientes` - Registrar paciente
- `GET /api/obrassociales` - Listar obras sociales

### Módulo de Urgencias (Enfermería)

**Características:**
- Formulario de triage responsive
- Registro de signos vitales:
  - Temperatura (°C)
  - Frecuencia Cardíaca (lpm)
  - Frecuencia Respiratoria (rpm)
  - Tensión Arterial (Sistólica/Diastólica)
- Clasificación de emergencia:
  - Crítica (Rojo) - Atención inmediata
  - Emergencia (Naranja) - Máx. 15 min
  - Urgencia (Amarillo) - Máx. 30 min
  - Menor (Verde) - Máx. 120 min
  - Sin Urgencia (Azul) - Máx. 240 min
- Cola de prioridad automática
- Verificación automática de pacientes existentes

**Endpoints:**
- `POST /api/urgencias` - Registrar nuevo ingreso
- `GET /api/urgencias/pendientes` - Obtener cola de espera

### Módulo de Atención (Médicos)

**Características:**
- Dashboard en tiempo real
- Estadísticas de pacientes por nivel
- Acción "Llamar Siguiente Paciente"
- Modal con resumen del ingreso:
  - Datos del paciente
  - Signos vitales
  - Informe de enfermería
- Registro de diagnóstico médico
- Finalización y alta del paciente
- Cancelación de atención (devuelve a cola)

**Endpoints:**
- `GET /api/urgencias/pendientes` - Ver cola de espera
- `POST /api/urgencias/{cuil}/reclamar` - Reclamar paciente
- `POST /api/atenciones` - Registrar atención
- `POST /api/urgencias/{cuil}/cancelar-atencion` - Cancelar atención

---

### Gestión de la Solución

#### Explorador de Soluciones

El Explorador de soluciones es tu herramienta principal:

**Acciones comunes:**
- **Agregar nuevo proyecto:** Click derecho en solución > Agregar > Nuevo proyecto
- **Agregar clase:** Click derecho en proyecto > Agregar > Clase
- **Establecer proyecto de inicio:** Click derecho en proyecto > Establecer como proyecto de inicio
- **Administrar paquetes NuGet:** Click derecho en proyecto > Administrar paquetes NuGet

### Troubleshooting Común en Visual Studio

#### Error "No se puede encontrar el proyecto"

**Solución:**
1. Verifica que todos los archivos `.csproj` existan
2. Abre el archivo `.sln` en un editor de texto y verifica las rutas
3. En Visual Studio: Click derecho en solución > "Volver a cargar solución"

#### Tests no aparecen en el Explorador de pruebas

**Soluciones:**
1. Compila la solución: `Ctrl + Shift + B`
2. Prueba > Actualizar (en el Explorador de pruebas)
3. Cierra y reabre el Explorador de pruebas
4. Verifica que el framework de pruebas (xUnit) esté instalado correctamente

#### Error de restauración de paquetes NuGet

**Solución:**
1. Click derecho en solución > Restaurar paquetes NuGet
2. Herramientas > NuGet > Administrador de paquetes NuGet > Configuración > Borrar caché
3. Elimina la carpeta `packages` y restaura nuevamente

#### El puerto está en uso

**Solución:**
1. Cambia el puerto en `Properties/launchSettings.json`
2. O detén el proceso que está usando el puerto:
```bash
netstat -ano | findstr :5284
taskkill /PID <número_de_proceso> /F
```

#### La aplicación no se detiene correctamente

**Solución:**
1. En lugar de cerrar el navegador, usa `Shift + F5` en Visual Studio
2. O click en el botón "Detener" rojo
3. Si persiste: Herramientas > Opciones > Depuración > General > "Detener todos los procesos cuando se detenga uno"

---

## Características Técnicas Destacadas

### Validación Centralizada de CUIL

**Backend (C#):**
- `Dominio/Validadores/ValidadorCUIL.cs` - Fuente única de verdad
- Valida formato (11 dígitos)
- Valida dígito verificador según algoritmo oficial
- Normalización automática (remueve guiones)

**Frontend (JavaScript):**
- `cliente/src/utils/cuilHelper.js` - Helper centralizado
- Funciones: `validateCuil()`, `formatCuil()`, `normalizeCuil()`
- Validación en tiempo real
- Mensajes de error claros

### Seguridad

**Autenticación:**
- JWT con claims personalizados
- Tokens firmados con clave secreta
- Expiración configurable
- Refresh tokens (futuro)

**Autorización:**
- Middleware de autorización por roles
- Endpoints protegidos con `[Authorize]`
- Validación de matrícula para operaciones críticas

**Validación:**
- FluentValidation en backend
- Validación de CUIL con dígito verificador
- Sanitización de inputs
- Queries parametrizadas (previene SQL Injection)

---

## CI/CD - GitHub Actions

El proyecto incluye integración continua automática.

### Workflows Configurados

**Backend Tests:** `.github/workflows/backend-tests.yml`
- Se ejecuta en cada push y pull request
- Ejecuta todas las pruebas (BDD + Unit)
- Verifica compilación exitosa
- Genera reporte de cobertura

## Estructura de Base de Datos

### Tablas Principales

**Pacientes**
- CUIL (PK)
- Nombre, Apellido
- Fecha de Nacimiento
- Dirección (Calle, Número, Localidad)
- Email, Teléfono
- Obra Social ID, Número de Afiliado

**Usuarios**
- Email (PK)
- Password Hash
- Nombre, Apellido, DNI, CUIL
- Matrícula
- Tipo Autoridad (0 = Médico, 1 = Enfermera)
- Fecha de Nacimiento, Teléfono

**Ingresos**
- ID (PK)
- CUIL Paciente (FK)
- DNI Enfermera (FK)
- Fecha Ingreso
- Nivel Emergencia
- Estado (Pendiente, En Proceso, Atendido)
- Signos Vitales (Temp, FC, FR, TA)
- Informe Enfermera

**Atenciones**
- ID (PK)
- Ingreso ID (FK)
- CUIL Paciente (FK)
- Matrícula Médico (FK)
- Fecha Atención
- Informe Médico

**Obras Sociales**
- ID (PK)
- Nombre
- Descripción

---

## Capturas de Pantalla

### Flujo de Enfermería

#### 1. Inicio de Sesión

<img width="1919" height="1079" alt="Login Enfermera" src="https://github.com/user-attachments/assets/7ff3312e-52b8-43d4-8425-604ee75b92c8" />

Popup de inicio exitoso:

<img width="1919" height="1079" alt="Login Exitoso" src="https://github.com/user-attachments/assets/e575f58a-4921-4709-aecc-dd9082d9eaa0" />

#### 2. Panel de Cola de Prioridad e Ingreso de Pacientes

<img width="1919" height="1079" alt="Panel Principal" src="https://github.com/user-attachments/assets/c93bb8dc-f9d4-4cc3-8af8-a455bb43e1b3" />

#### 2.1 Formulario de Nuevo Ingreso

<img width="1919" height="1079" alt="Formulario Ingreso" src="https://github.com/user-attachments/assets/aa6dfdf2-2ea8-4a57-9896-1ac387295234" />

#### 2.1.1 Carga de Paciente Nuevo

<img width="1919" height="1079" alt="Paciente Nuevo 1" src="https://github.com/user-attachments/assets/25f9424b-f156-4589-b770-8ad4b4008b24" />
<img width="1919" height="1079" alt="Paciente Nuevo 2" src="https://github.com/user-attachments/assets/b20f0b3b-465f-4dc5-a95b-023b1f7d5376" />
<img width="1919" height="1079" alt="Paciente Nuevo 3" src="https://github.com/user-attachments/assets/40fcf105-cd4a-4142-a757-5dfecc6f8531" />

#### 2.1.2 Reconocimiento de Paciente Nuevo

<img width="1919" height="1079" alt="Reconoce Paciente" src="https://github.com/user-attachments/assets/114aa451-02dd-4da5-be99-fa8070c1f183" />

#### 2.1.2.1 Formulario de Datos del Paciente Nuevo

<img width="1919" height="1079" alt="Formulario Paciente 1" src="https://github.com/user-attachments/assets/d910e10a-88cf-4e1e-b50e-589942ddd6ee" />
<img width="1919" height="1079" alt="Formulario Paciente 2" src="https://github.com/user-attachments/assets/7cf1c880-7b96-4bcc-bcb5-7682b78dd19c" />

#### 2.2 Ingreso Exitoso en Cola de Prioridad

<img width="1919" height="1079" alt="Ingreso Exitoso" src="https://github.com/user-attachments/assets/c939f9d5-a704-4b54-ab81-876d6e7d2a19" />

#### 3. Ingreso de Paciente Existente con Mayor Prioridad

<img width="1919" height="1079" alt="Paciente Existente 1" src="https://github.com/user-attachments/assets/244b4eee-2395-4c7e-87f2-987ec82d414a" />
<img width="1919" height="1079" alt="Paciente Existente 2" src="https://github.com/user-attachments/assets/31e42ff6-0c85-4d2e-bd37-1287c14163fb" />
<img width="1919" height="1079" alt="Paciente Existente 3" src="https://github.com/user-attachments/assets/d1538ad6-00c8-4f0a-8b9d-6da577f50831" />

#### 3.1 Cola de Prioridad Ordenada

<img width="1919" height="1079" alt="Cola Ordenada 1" src="https://github.com/user-attachments/assets/5657c99f-5995-459c-9b55-7b5b4ba39360" />
<img width="1919" height="1079" alt="Cola Ordenada 2" src="https://github.com/user-attachments/assets/dad96b5d-ab0e-40d6-97d1-c7bf7fc23967" />

### Flujo Médico

#### 1. Inicio de Sesión

<img width="1919" height="1079" alt="Login Médico" src="https://github.com/user-attachments/assets/560d865d-39d6-41d0-8b17-ff9c3ba902c5" />

Popup de inicio exitoso:

<img width="1919" height="1079" alt="Login Médico Exitoso" src="https://github.com/user-attachments/assets/fcd6c3c4-33f0-4930-a20c-232a7effce16" />

#### 2. Panel Médico con Cola de Prioridad

<img width="1919" height="1079" alt="Panel Médico 1" src="https://github.com/user-attachments/assets/8b6dcf3a-0058-4eef-bfe1-f8948c6306fd" />
<img width="1919" height="1079" alt="Panel Médico 2" src="https://github.com/user-attachments/assets/6d175fa3-79c5-4ce5-974f-4b3dbd9929c8" />

#### 3. Llamar Paciente para Atención

<img width="1919" height="1079" alt="Llamar Paciente" src="https://github.com/user-attachments/assets/0b5b12c2-80bf-4e46-80ba-4d349f86e44e" />

#### 3.1 Modal de Registro de Informe

<img width="1919" height="1079" alt="Modal Informe" src="https://github.com/user-attachments/assets/61c88c12-e406-470a-8c16-02a61a339a9a" />

#### 3.2 Confirmación de Registro y Alta del Paciente

<img width="1919" height="1079" alt="Confirmación Alta 1" src="https://github.com/user-attachments/assets/a9828d18-809c-43fd-8a42-e7d8534a8580" />
<img width="1919" height="1029" alt="Confirmación Alta 2" src="https://github.com/user-attachments/assets/fe0ee962-9a49-47b1-8853-d086cc82594f" />

