using Aplicacion;
using Aplicacion.Intefaces;
using Dominio.Interfaces;
using Infraestructura;
using Infraestructura.Memory;
using Microsoft.OpenApi;
using Scalar.AspNetCore; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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

//builder.Services.AddSingleton<IRepositorioUrgencias, RepositorioUrgenciasMemoria>();
builder.Services.AddScoped<IRepositorioUrgencias, RepositorioUrgenciasADO>();
builder.Services.AddScoped<IRepositorioPacientes, RepositorioPacientesADO>();
//builder.Services.AddSingleton<IRepositorioPacientes, DBPruebaMemoria>();
//builder.Services.AddSingleton<IRepositorioObraSocial, RepositorioObraSocialMemoria>();
builder.Services.AddScoped<IRepositorioObraSocial, RepositorioObraSocialADO>();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();

builder.Services.AddScoped<IServicioUrgencias, ServicioUrgencias>();
builder.Services.AddScoped<IServicioPacientes, ServicioPacientes>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioAtencion, ServicioAtencion>();

var app = builder.Build();


// Poblar datos de prueba al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var repositorioPacientes = services.GetRequiredService<IRepositorioPacientes>();
        var repositorioObraSocial = services.GetRequiredService<IRepositorioObraSocial>();

        Web.DataSeeder.SeedData(repositorioPacientes, repositorioObraSocial);

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Datos de prueba inicializados correctamente");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar datos de prueba");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();

    app.MapScalarApiReference();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); 
app.UseAuthorization();
app.MapControllers(); 

app.Run();