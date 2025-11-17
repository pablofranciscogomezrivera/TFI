using Aplicacion;
using Aplicacion.Intefaces;
using Dominio.Interfaces;
using Infraestructura;
using Microsoft.OpenApi;
using Scalar.AspNetCore; // Importante para Scalar

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar API Controllers
builder.Services.AddControllers();

// 2. Configurar Swagger/OpenAPI (necesario para ambos)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Esto define el documento "v1" que Scalar no encontraba
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFI - API de Urgencias", Version = "v1" });
});

// 3. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Registrar Repositorios (Singleton para estado en memoria)
builder.Services.AddSingleton<IRepositorioUrgencias, RepositorioUrgenciasMemoria>();
builder.Services.AddSingleton<IRepositorioPacientes, DBPruebaMemoria>();
builder.Services.AddSingleton<IRepositorioObraSocial, RepositorioObraSocialMemoria>();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();

// 5. Registrar Servicios (Scoped)
builder.Services.AddScoped<IServicioUrgencias, ServicioUrgencias>();
builder.Services.AddScoped<IServicioPacientes, ServicioPacientes>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioAtencion, ServicioAtencion>();

var app = builder.Build();

// 6. Configurar el Pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    // A. Genera el archivo swagger.json. DEBE IR PRIMERO.
    app.UseSwagger();

    // B. Sirve la UI de Swagger en /swagger
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital Urgencias API v1"));

    // C. Sirve la UI de Scalar en /scalar
    app.MapScalarApiReference();
}
else
{
    // En producción, no expones Swagger/Scalar
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Habilita CORS
app.UseAuthorization();
app.MapControllers(); // Mapea tus Controllers

app.Run();