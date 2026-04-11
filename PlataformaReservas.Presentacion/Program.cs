using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlataformaReservas.Aplicacion.DTOs;
using PlataformaReservas.Aplicacion.Interfaces;
using PlataformaReservas.Aplicacion.Services;
using PlataformaReservas.Dominio.Repositorios;
using PlataformaReservas.Infraestructura.Persistencia;
using PlataformaReservas.Infraestructura.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias (REPOSITORIOS)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPropiedadRepository, PropiedadRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IFechaBloqueadaRepository, FechaBloqueadaRepository>();
builder.Services.AddScoped<IResenaRepository, ResenaRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();

// 3. Inyección de Dependencias (SERVICIOS)
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPropiedadService, PropiedadService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IFechaBloqueadaService, FechaBloqueadaService>();
builder.Services.AddScoped<IResenaService, ResenaService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

// 4. Registro de los Validadores (FluentValidation) automáticamente
builder.Services.AddValidatorsFromAssemblyContaining<CrearPropiedadDto>();


// 5. Configuración de JWT.
var jwtKey = builder.Configuration["Jwt:Key"] ?? "UnaClaveSuperSecretaYMuyLargaParaQueNadieLaAdivine123456789";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false, 
            ValidateAudience = false 
        };
    });

// 6. Habilitar los Controladores (Endpoints)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del flujo HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Primero Authentication, luego Authorization
app.UseAuthentication();
app.UseAuthorization();

// Mapear los controladores
app.MapControllers();

app.Run();