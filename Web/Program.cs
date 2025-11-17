using Aplicacion;
using Aplicacion.Intefaces;
using Dominio.Interfaces;
using Infraestructura;
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

builder.Services.AddSingleton<IRepositorioUrgencias, RepositorioUrgenciasMemoria>();
builder.Services.AddSingleton<IRepositorioPacientes, DBPruebaMemoria>();
builder.Services.AddSingleton<IRepositorioObraSocial, RepositorioObraSocialMemoria>();
builder.Services.AddSingleton<IRepositorioUsuario, RepositorioUsuarioMemoria>();

builder.Services.AddScoped<IServicioUrgencias, ServicioUrgencias>();
builder.Services.AddScoped<IServicioPacientes, ServicioPacientes>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioAtencion, ServicioAtencion>();

var app = builder.Build();

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