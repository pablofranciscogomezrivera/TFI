# 🎨 Documentación con Scalar

## ¿Qué es Scalar?

**Scalar** es una herramienta moderna de documentación de APIs que genera interfaces interactivas a partir de especificaciones OpenAPI (anteriormente conocido como Swagger). Es la evolución natural de Swagger UI, ofreciendo una experiencia de usuario superior.

## 🌟 Ventajas de Scalar

### Diseño Moderno
- Interfaz limpia y profesional
- Dark mode automático según preferencias del sistema
- Diseño responsive para desktop, tablet y mobile
- Tipografía optimizada para legibilidad

### Rendimiento Superior
- Carga instantánea de la documentación
- Navegación fluida sin retrasos
- Optimizado para APIs con cientos de endpoints

### Búsqueda Avanzada
- Búsqueda en tiempo real
- Encuentra endpoints, modelos y parámetros instantáneamente
- Resaltado de resultados
- Navegación por teclado

### Ejemplos de Código
Genera automáticamente ejemplos en múltiples lenguajes:
- cURL
- C# (HttpClient)
- JavaScript (Fetch, Axios)
- Python (Requests)
- PHP
- Ruby
- Go
- Java
- Y más...

### Testing Interactivo
- Ejecuta requests directamente desde la documentación
- Edita parámetros y body en tiempo real
- Ve las respuestas formateadas
- Historial de requests

### Características Adicionales
- Exportación de colecciones
- Compartir enlaces a endpoints específicos
- Comentarios y anotaciones
- Temas personalizables

## 🚀 Cómo Acceder

Una vez que la aplicación esté corriendo, abre tu navegador en:

```
http://localhost:5000/scalar
```

O si estás usando HTTPS:

```
https://localhost:5001/scalar
```

## 📖 Cómo Usar Scalar

### 1. Explorar Endpoints

En el panel izquierdo verás todos los endpoints organizados por tags:
- **Auth**: Autenticación y registro
- **Pacientes**: Gestión de pacientes
- **Urgencias**: Módulo de urgencias
- **Atenciones**: Registro de atenciones médicas

### 2. Ver Detalles de un Endpoint

Haz clic en cualquier endpoint para ver:
- Descripción completa
- Parámetros requeridos y opcionales
- Estructura del request body
- Posibles respuestas con códigos HTTP
- Modelos de datos
- Ejemplos de request y response

### 3. Probar un Endpoint

1. Selecciona el endpoint que deseas probar
2. Completa los parámetros requeridos
3. Edita el body si es necesario
4. Haz clic en "Send Request"
5. Ve la respuesta en tiempo real

### 4. Generar Código

1. Selecciona un endpoint
2. Haz clic en el selector de lenguaje (por defecto: cURL)
3. Elige tu lenguaje preferido (C#, JavaScript, Python, etc.)
4. Copia el código generado
5. Pégalo en tu aplicación

### 5. Buscar

Usa la barra de búsqueda en la parte superior para:
- Buscar endpoints por nombre o método
- Encontrar modelos de datos
- Localizar parámetros específicos

## 🎯 Ejemplos de Uso

### Ejemplo 1: Registrar un Usuario

1. Abre Scalar en http://localhost:5000/scalar
2. En el panel izquierdo, navega a **Auth → POST /api/auth/registrar**
3. En el panel derecho, verás el schema del request:
   ```json
   {
     "email": "string",
     "password": "string",
     "tipoAutoridad": 0
   }
   ```
4. Completa los campos
5. Haz clic en "Send Request"
6. Ve la respuesta con el usuario creado

### Ejemplo 2: Generar Código para C#

1. Selecciona el endpoint **POST /api/urgencias**
2. Haz clic en el selector de lenguaje
3. Selecciona "C# - HttpClient"
4. Verás algo como:

```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;

var client = new HttpClient();
var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/api/urgencias");
request.Headers.Add("X-Enfermera-Matricula", "ENF123");

var body = new {
    cuilPaciente = "20-30123456-3",
    informe = "Dolor de pecho",
    temperatura = 37.5,
    nivelEmergencia = 1,
    frecuenciaCardiaca = 90,
    frecuenciaRespiratoria = 20,
    frecuenciaSistolica = 130,
    frecuenciaDiastolica = 85
};

request.Content = new StringContent(
    JsonSerializer.Serialize(body),
    Encoding.UTF8,
    "application/json"
);

var response = await client.SendAsync(request);
var content = await response.Content.ReadAsStringAsync();
```

## 🛠️ Configuración

La configuración de Scalar se encuentra en `Webb/Program.cs`:

```csharp
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Hospital Urgencias API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});
```

### Opciones de Personalización

#### Temas Disponibles
- `ScalarTheme.Purple` (por defecto)
- `ScalarTheme.Blue`
- `ScalarTheme.Green`
- `ScalarTheme.Orange`
- `ScalarTheme.Pink`
- `ScalarTheme.Default`
- `ScalarTheme.Dark`
- `ScalarTheme.Light`

#### Clientes HTTP Predeterminados
- `ScalarClient.HttpClient` (C#)
- `ScalarClient.Fetch` (JavaScript)
- `ScalarClient.Axios` (JavaScript)
- `ScalarClient.Xhr` (JavaScript)

#### Ejemplo de Configuración Personalizada

```csharp
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Mi API")
        .WithTheme(ScalarTheme.Blue)
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
        .WithModels(true)
        .WithDefaultOpenAllTags(false);
});
```

## 🔧 Troubleshooting

### Scalar no carga

**Problema**: Scalar muestra "document v1 not found"

**Solución**:
1. Verifica que `app.UseSwagger()` esté antes de `app.MapScalarApiReference()`
2. Asegúrate de usar la URL correcta: `http://localhost:5000/scalar` (sin `/v1`)
3. Verifica que estés en modo Development

### No se ven todos los endpoints

**Solución**:
1. Verifica que tus controllers tengan el atributo `[ApiController]`
2. Asegúrate de que los métodos sean públicos
3. Verifica que tengan atributos HTTP (`[HttpGet]`, `[HttpPost]`, etc.)

### Los ejemplos no funcionan

**Solución**:
1. Verifica que la API esté corriendo
2. Asegúrate de que no haya errores de CORS
3. Revisa la consola del navegador para errores

## 📚 Recursos Adicionales

- [Documentación Oficial de Scalar](https://github.com/scalar/scalar)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Scalar.AspNetCore en NuGet](https://www.nuget.org/packages/Scalar.AspNetCore/)

## 💡 Tips y Trucos

### Navegación por Teclado
- `Ctrl/Cmd + K`: Abrir búsqueda rápida
- `Esc`: Cerrar modales
- `↑` `↓`: Navegar por resultados de búsqueda

### Enlaces Directos
Puedes compartir enlaces a endpoints específicos:
```
http://localhost:5000/scalar#/operations/post-api-urgencias
```

### Modo Dark/Light
Scalar detecta automáticamente las preferencias del sistema. También puedes cambiar el tema manualmente en la configuración.

### Exportar Colección
Puedes exportar toda la API como colección para Postman, Insomnia u otras herramientas.

## 🎓 Comparación con Swagger UI

| Característica | Scalar | Swagger UI |
|---------------|--------|------------|
| Diseño | ✨ Moderno | ⚙️ Clásico |
| Rendimiento | 🚀 Rápido | 🐢 Lento con muchos endpoints |
| Dark Mode | ✅ Automático | ⚠️ Manual |
| Ejemplos de Código | 🌈 Múltiples lenguajes | 📝 Solo cURL |
| Búsqueda | 🔍 Avanzada | 🔎 Básica |
| UI/UX | 🎨 Intuitiva | 📋 Funcional |
| Responsive | 📱 Excelente | 💻 Desktop-first |
| Personalización | 🎭 Temas y estilos | ⚙️ Limitada |

---

## ✅ Conclusión

Scalar proporciona una experiencia moderna y profesional para documentar y explorar tu API. Su interfaz intuitiva, rendimiento superior y características avanzadas lo convierten en la mejor opción para documentación de APIs en 2025.

**¿Listo para empezar?** Abre tu navegador en http://localhost:5000/scalar y comienza a explorar la API.