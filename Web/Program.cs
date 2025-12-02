using Aplicacion;
using Aplicacion.Intefaces;
using Dominio.Interfaces;
using Infraestructura;
using Infraestructura.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFI - API de Urgencias", Version = "v1" });
});

//Configurar CORS
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
//builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuarioADO>();
builder.Services.AddScoped<IRepositorioPersonal, RepositorioPersonalADO>();
builder.Services.AddScoped<IServicioUrgencias, ServicioUrgencias>();
builder.Services.AddScoped<IServicioPacientes, ServicioPacientes>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioAtencion, ServicioAtencion>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Obtenemos todos los servicios necesarios
        var repositorioPacientes = services.GetRequiredService<IRepositorioPacientes>();
        var repositorioObraSocial = services.GetRequiredService<IRepositorioObraSocial>();
        var servicioAutenticacion = services.GetRequiredService<IServicioAutenticacion>();
        var configuration = services.GetRequiredService<IConfiguration>();

        Web.DataSeeder.SeedData(
            repositorioPacientes,
            repositorioObraSocial,
            servicioAutenticacion,
            configuration
        );

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Datos de prueba inicializados correctamente (Pacientes, Obras Sociales y Usuarios).");
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 

app.Run();