using Aplicacion;
using Aplicacion.Intefaces;
using Dominio.Interfaces;
using Infraestructura;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure API Controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Hospital Urgencias API", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Repositories as Singleton (to maintain state in memory)
builder.Services.AddSingleton<IRepositorioUrgencias, RepositorioUrgenciasMemoria>();
builder.Services.AddSingleton<IRepositorioPacientes, DBPruebaMemoria>();
builder.Services.AddSingleton<IRepositorioObraSocial, RepositorioObraSocialMemoria>();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();

// Register Services as Scoped
builder.Services.AddScoped<IServicioUrgencias, ServicioUrgencias>();
builder.Services.AddScoped<IServicioPacientes, ServicioPacientes>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioAtencion, ServicioAtencion>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors("AllowAll");

// Map API Controllers
app.MapControllers();
app.Run();