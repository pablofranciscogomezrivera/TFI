# Hospital Urgencias API - Documentación

## Descripción General

API REST para el sistema de gestión de urgencias hospitalarias. Incluye módulos de autenticación, registro de pacientes, gestión de urgencias y registro de atenciones médicas.

## URL Base

```
http://localhost:5000/api
https://localhost:5001/api
```

## Documentación Interactiva

Una vez que la aplicación esté corriendo, puedes acceder a la documentación interactiva en:

### Scalar (Recomendado - Moderna y Rápida)
```
http://localhost:5000/scalar/v1
https://localhost:5001/scalar/v1
```

### Swagger UI (Clásica)
```
http://localhost:5000/swagger
https://localhost:5001/swagger
```

**Recomendamos usar Scalar** por su interfaz moderna, mejor rendimiento y características adicionales como:
- Dark mode automático
- Búsqueda mejorada
- Mejor visualización de ejemplos
- Exportación de código en múltiples lenguajes

---

## Endpoints

### 1. Autenticación

#### POST /api/auth/registrar
Registra un nuevo usuario en el sistema.

**Request Body:**
```json
{
  "email": "doctor@hospital.com",
  "password": "SecurePassword123",
  "tipoAutoridad": 0  // 0 = Medico, 1 = Enfermera
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "email": "doctor@hospital.com",
  "tipoAutoridad": 0
}
```

**Errores posibles:**
- 400: Email inválido, contraseña menor a 8 caracteres, email ya registrado

---

#### POST /api/auth/login
Inicia sesión en el sistema.

**Request Body:**
```json
{
  "email": "doctor@hospital.com",
  "password": "SecurePassword123"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "email": "doctor@hospital.com",
  "tipoAutoridad": 0
}
```

**Errores posibles:**
- 401: Usuario o contraseña inválidos

---

### 2. Pacientes

#### POST /api/pacientes
Registra un nuevo paciente en el sistema. **Requiere rol de Enfermera**.

**Request Body:**
```json
{
  "cuil": "20-30123456-3",
  "nombre": "Juan",
  "apellido": "Pérez",
  "calle": "San Martín",
  "numero": 123,
  "localidad": "San Miguel de Tucumán",
  "obraSocialId": 1,  // Opcional
  "numeroAfiliado": "12345"  // Opcional (requerido si obraSocialId está presente)
}
```

**Response (201 Created):**
```json
{
  "cuil": "20-30123456-3",
  "nombre": "Juan",
  "apellido": "Pérez",
  "domicilio": {
    "calle": "San Martín",
    "numero": 123,
    "localidad": "San Miguel de Tucumán"
  },
  "afiliado": {
    "numeroAfiliado": "12345",
    "obraSocial": "OSDE"
  }
}
```

**Errores posibles:**
- 400: CUIL inválido, campos mandatorios faltantes, obra social inexistente, paciente no afiliado

---

### 3. Urgencias

#### POST /api/urgencias
Registra una nueva urgencia. **Requiere rol de Enfermera**.

**Headers:**
```
X-Enfermera-Matricula: ENF123
```

**Request Body:**
```json
{
  "cuilPaciente": "20-30123456-3",
  "informe": "Paciente con dolor torácico intenso y dificultad respiratoria",
  "temperatura": 38.2,
  "nivelEmergencia": 0,  // 0=CRITICA, 1=EMERGENCIA, 2=URGENCIA, 3=URGENCIA_MENOR, 4=SIN_URGENCIA
  "frecuenciaCardiaca": 105,
  "frecuenciaRespiratoria": 24,
  "frecuenciaSistolica": 145,
  "frecuenciaDiastolica": 92
}
```

**Response (201 Created):**
```json
{
  "message": "Urgencia registrada exitosamente"
}
```

**Errores posibles:**
- 400: Informe mandatorio, datos inválidos

---

#### GET /api/urgencias/lista-espera
Obtiene la lista de pacientes en espera, ordenados por prioridad.

**Response (200 OK):**
```json
[
  {
    "cuilPaciente": "20-30123456-3",
    "nombrePaciente": "Juan",
    "apellidoPaciente": "Pérez",
    "informeInicial": "Paciente con dolor torácico intenso...",
    "nivelEmergencia": 0,
    "estado": 0,  // 0=PENDIENTE, 1=EN_PROCESO, 2=FINALIZADO
    "fechaIngreso": "2025-11-17T14:30:00Z",
    "signosVitales": {
      "temperatura": 38.2,
      "frecuenciaCardiaca": 105,
      "frecuenciaRespiratoria": 24,
      "tensionSistolica": 145,
      "tensionDiastolica": 92
    }
  }
]
```

---

#### POST /api/urgencias/reclamar
Reclama el siguiente paciente de la lista de espera. **Requiere rol de Médico**.

**Headers:**
```
X-Doctor-Matricula: MP12345
```

**Response (200 OK):**
```json
{
  "cuilPaciente": "20-30123456-3",
  "nombrePaciente": "Juan",
  "apellidoPaciente": "Pérez",
  "informeInicial": "Paciente con dolor torácico intenso...",
  "nivelEmergencia": 0,
  "estado": 1,  // EN_PROCESO
  "fechaIngreso": "2025-11-17T14:30:00Z",
  "signosVitales": {
    "temperatura": 38.2,
    "frecuenciaCardiaca": 105,
    "frecuenciaRespiratoria": 24,
    "tensionSistolica": 145,
    "tensionDiastolica": 92
  }
}
```

**Errores posibles:**
- 400: No hay pacientes en la lista de espera, doctor no proporcionado

---

### 4. Atenciones

#### POST /api/atenciones/{cuilPaciente}
Registra la atención médica de un paciente. **Requiere rol de Médico**.

**Path Parameters:**
- `cuilPaciente`: CUIL del paciente a atender

**Headers:**
```
X-Doctor-Matricula: MP12345
```

**Request Body:**
```json
{
  "informeMedico": "Diagnóstico: Síndrome coronario agudo. ECG con elevación ST. Tratamiento: AAS 300mg, Clopidogrel 600mg, Heparina IV. Derivar urgente a hemodinamia."
}
```

**Response (200 OK):**
```json
{
  "cuilPaciente": "20-30123456-3",
  "nombrePaciente": "Juan",
  "apellidoPaciente": "Pérez",
  "doctor": "Doctor Sistema",
  "matriculaDoctor": "MP12345",
  "informeCompleto": "Paciente con dolor torácico intenso...\n\nInforme médico: Diagnóstico: Síndrome coronario agudo..."
}
```

**Errores posibles:**
- 400: Informe mandatorio, ingreso no está en proceso
- 404: No se encontró ingreso en proceso para el paciente

---

## Flujo de Trabajo Típico

### 1. Enfermera registra un paciente nuevo
```
POST /api/pacientes
```

### 2. Enfermera registra la urgencia
```
POST /api/urgencias
```

### 3. Médico consulta la lista de espera
```
GET /api/urgencias/lista-espera
```

### 4. Médico reclama el siguiente paciente
```
POST /api/urgencias/reclamar
```

### 5. Médico registra la atención
```
POST /api/atenciones/{cuilPaciente}
```

---

## Códigos de Estado HTTP

- **200 OK**: Operación exitosa
- **201 Created**: Recurso creado exitosamente
- **400 Bad Request**: Error en los datos enviados
- **401 Unauthorized**: Credenciales inválidas
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error del servidor

---

## Notas Importantes

1. **Autenticación**: Actualmente la API usa headers simples (`X-Enfermera-Matricula`, `X-Doctor-Matricula`). En producción, se recomienda implementar JWT tokens.

2. **CORS**: Está configurado para permitir todas las origins en desarrollo. Ajustar en producción.

3. **Persistencia**: Los datos se almacenan en memoria. Al reiniciar el servidor, se pierden todos los datos.

4. **Prioridad de Urgencias**:
   - 0 = CRITICA (máxima prioridad)
   - 1 = EMERGENCIA
   - 2 = URGENCIA
   - 3 = URGENCIA_MENOR
   - 4 = SIN_URGENCIA (mínima prioridad)

5. **Estados de Ingreso**:
   - 0 = PENDIENTE (en lista de espera)
   - 1 = EN_PROCESO (siendo atendido)
   - 2 = FINALIZADO (atención completada)

---

## Ejemplo de Uso con cURL

### Registrar usuario
```bash
curl -X POST http://localhost:5000/api/auth/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "email": "enfermera@hospital.com",
    "password": "Password123",
    "tipoAutoridad": 1
  }'
```

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "enfermera@hospital.com",
    "password": "Password123"
  }'
```

### Registrar paciente
```bash
curl -X POST http://localhost:5000/api/pacientes \
  -H "Content-Type: application/json" \
  -d '{
    "cuil": "20-30123456-3",
    "nombre": "Juan",
    "apellido": "Pérez",
    "calle": "San Martín",
    "numero": 123,
    "localidad": "Tucumán"
  }'
```

### Registrar urgencia
```bash
curl -X POST http://localhost:5000/api/urgencias \
  -H "Content-Type: application/json" \
  -H "X-Enfermera-Matricula: ENF123" \
  -d '{
    "cuilPaciente": "20-30123456-3",
    "informe": "Dolor de pecho",
    "temperatura": 37.5,
    "nivelEmergencia": 1,
    "frecuenciaCardiaca": 90,
    "frecuenciaRespiratoria": 20,
    "frecuenciaSistolica": 130,
    "frecuenciaDiastolica": 85
  }'
```

### Ver lista de espera
```bash
curl -X GET http://localhost:5000/api/urgencias/lista-espera
```

### Reclamar paciente
```bash
curl -X POST http://localhost:5000/api/urgencias/reclamar \
  -H "X-Doctor-Matricula: MP12345"
```

### Registrar atención
```bash
curl -X POST http://localhost:5000/api/atenciones/20-30123456-3 \
  -H "Content-Type: application/json" \
  -H "X-Doctor-Matricula: MP12345" \
  -d '{
    "informeMedico": "Diagnóstico: Angina de pecho. Tratamiento: Nitroglicerina."
  }'
```

---

## Ejecutar la Aplicación

```bash
cd Webb
dotnet run
```

La API estará disponible en:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: http://localhost:5000/swagger