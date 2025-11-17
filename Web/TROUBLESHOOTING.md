# Troubleshooting - Guía de Solución de Problemas

## Problema: Scalar muestra "document v1 not found"

### Solución
Asegúrate de que la configuración en `Program.cs` esté correcta:

```csharp
if (app.Environment.IsDevelopment())
{
    // OpenAPI/Swagger middleware (necesario para generar la especificación)
    app.UseSwagger();
    
    // Scalar - Documentación de API moderna e interactiva
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Hospital Urgencias API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}
```

### URL Correcta

- ✅ **Scalar**: `http://localhost:5000/scalar`
- ❌ **NO usar**: `http://localhost:5000/scalar/v1` (esto causará el error)

### Verificación
1. Asegúrate de que `app.UseSwagger()` esté ANTES de `app.MapScalarApiReference()`
2. Usa la URL correcta sin `/v1`: `http://localhost:5000/scalar`
3. Verifica que el servidor esté en modo Development

---

## Problema: Puerto ya en uso

### Síntomas
```
System.Net.Sockets.SocketException: Address already in use
```

### Solución 1: Cambiar puerto
```bash
dotnet run --urls "http://localhost:5555"
```

### Solución 2: Matar proceso
```bash
# Linux/Mac
lsof -ti:5000 | xargs kill -9

# Windows (PowerShell)
Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess | Stop-Process
```

---

## Problema: Certificado HTTPS no confiable

### Síntomas
```
SSL connection error
Certificate not trusted
```

### Solución
```bash
dotnet dev-certs https --trust
```

Si el problema persiste, usa HTTP en desarrollo:
```bash
dotnet run --urls "http://localhost:5000"
```

---

## Problema: Tests fallan después de cambios

### Solución
1. Limpiar y reconstruir:
```bash
dotnet clean
dotnet build
dotnet test
```

2. Si persiste, verificar que los repositorios estén correctamente inyectados en los tests

---

## Problema: "Cannot resolve service" en runtime

### Síntomas
```
InvalidOperationException: Unable to resolve service for type
```

### Solución
Verificar que todos los servicios estén registrados en `Program.cs`:

```csharp
// Repositorios como Singleton
builder.Services.AddSingleton<IRepositorioUrgencias, RepositorioUrgenciasMemoria>();
builder.Services.AddSingleton<IRepositorioPacientes, DBPruebaMemoria>();
builder.Services.AddSingleton<IRepositorioObraSocial, RepositorioObraSocialMemoria>();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();

// Servicios como Scoped
builder.Services.AddScoped<IServicioUrgencias, ServicioUrgencias>();
builder.Services.AddScoped<IServicioPacientes, ServicioPacientes>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioAtencion, ServicioAtencion>();
```

---

## Problema: CORS bloqueando requests

### Síntomas
```
Access to fetch at 'http://localhost:5000/api/...' has been blocked by CORS policy
```

### Solución
Verificar configuración CORS en `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ...

app.UseCors("AllowAll");
```

Para producción, especificar origins:
```csharp
policy.WithOrigins("https://mi-frontend.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

---

## Problema: Data se pierde al reiniciar

### Explicación
Los datos están en memoria. Esto es normal en el ambiente de desarrollo actual.

### Solución temporal
Crear script de inicialización con datos de prueba.

### Solución permanente
Implementar persistencia real (SQL Server, PostgreSQL, etc.)

---

## Problema: Scalar no muestra todos los endpoints

### Solución
1. Verificar que los Controllers tengan atributos correctos:
```csharp
[ApiController]
[Route("api/[controller]")]
public class MiController : ControllerBase
```

2. Asegurarse de que el método sea público y tenga atributos HTTP:
```csharp
[HttpPost]
public IActionResult MiMetodo()
```

3. Verificar que `AddSwaggerGen()` esté correctamente configurado en `Program.cs`

---

## Problema: Build lento

### Solución
Limpiar archivos temporales:
```bash
dotnet clean
rm -rf */bin */obj
dotnet restore
dotnet build
```

---

## Problema: Hot reload no funciona

### Solución
```bash
# Usar watch mode
dotnet watch run
```

---

## Verificación Rápida del Sistema

Ejecuta estos comandos para verificar que todo funciona:

```bash
# 1. Build
cd Webb
dotnet build

# 2. Tests
cd ..
dotnet test

# 3. Run
cd Webb
dotnet run
```

Deberías ver:
- ✅ Build succeeded
- ✅ Passed! - Failed: 0, Passed: 89
- ✅ Now listening on: http://localhost:5000

---

## Logs y Debugging

### Ver logs detallados
```bash
dotnet run --verbosity detailed
```

### Nivel de log en appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## Recursos Adicionales

- [ASP.NET Core Troubleshooting](https://docs.microsoft.com/en-us/aspnet/core/test/troubleshoot)
- [Scalar Documentation](https://github.com/scalar/scalar)
- [OpenAPI Specification](https://swagger.io/specification/)

---

## Contacto

Si el problema persiste:
1. Verifica los logs de error completos
2. Asegúrate de tener .NET 8.0 SDK
3. Intenta con `dotnet clean` y `dotnet build`
4. Revisa que todas las dependencias estén instaladas
