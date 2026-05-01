using Microsoft.EntityFrameworkCore;
using Sodum.API.Data;
using Sodum.API.Services;

var builder = WebApplication.CreateBuilder(args);

// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<AppDbContext>(options =>
  //  options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configurando o serviço do Last.fm usando o padrão de HttpClient tipado
builder.Services.AddHttpClient<ILastFmService, LastFmService>();
builder.Services.AddHttpClient<ISteamService, SteamService>();

builder.Services.AddHttpClient<IRecommendationEngine, LlmRecommendationEngine>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SodumPolicy", policy =>
    {
        policy.WithOrigins(
                    "sodum-ui.vercel.app",
                    "https://sodum-api.onrender.com",
                    "https://www.sodum.com",           // Domínio oficial de produção do Sodum
                    "https://sodum.com",               // Variação sem www
                    "http://localhost:3000",           // Testes locais (ex: React/Next.js)
                    "http://localhost:5173"            // Testes locais (ex: Vite)
               )
              .WithMethods("GET", "POST", "PUT", "DELETE") // Métodos estritamente necessários
              .WithHeaders("Content-Type", "Authorization") // Cabeçalhos permitidos
              .AllowCredentials(); // Segurança para cookies e tokens de autenticação
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
