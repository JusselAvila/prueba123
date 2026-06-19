using EventosAPI.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EventosAPI.Gestion;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de JWT
var jwtKey = builder.Configuration["Jwt:Key"] 
             ?? throw new InvalidOperationException("La clave JWT no está configurada.");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Configuración de CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers(); 
builder.Services.AddScoped<AuthGestion>();
builder.Services.AddScoped<EventoGestion>();
builder.Services.AddScoped<InscripcionGestion>();
builder.Services.AddScoped<TokenGestion>();

builder.Services.AddOpenApi();

var app = builder.Build();

// --- Migraciones Automáticas ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Intentar conectar hasta 10 veces
    int retries = 10;
    while (retries > 0)
    {
        try {
            dbContext.Database.Migrate();
            break; // Si tiene éxito, salimos del bucle
        }
        catch (Exception ex) {
            retries--;
            Console.WriteLine($"Base de datos no lista, reintentando... ({retries} intentos restantes). Error: {ex.Message}");
            System.Threading.Thread.Sleep(3000); 
        }
    }
}
// Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 

app.Run();