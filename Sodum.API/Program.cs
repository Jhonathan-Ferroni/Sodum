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
builder.Services.AddHttpClient<ILastFmServiceSearch, LastFmServiceSearch>();

builder.Services.AddHttpClient<IRecommendationEngine, LlmRecommendationEngine>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SodumPolicy", policy => // Criando a política com este nome
    {
        policy.WithOrigins(
                    "https://sodum.vercel.app",        // CORRIGIDO: URL exata do print com https://
                    "https://sodum-ui.vercel.app",     // Mantido caso você use este alias no Vercel
                    "https://sodum-api.onrender.com",
                    "https://www.sodum.com",           // Domínio oficial de produção do Sodum
                    "https://sodum.com",               // Variação sem www
                    "http://localhost:3000",           // Testes locais (ex: React/Next.js)
                    "http://localhost:5173"            // Testes locais (ex: Vite)
               )
              // Adicionado o OPTIONS que é vital para o preflight do navegador
              .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS") 
              // AllowAnyHeader é mais seguro aqui para não bloquear Accept, Origin, etc.
              .AllowAnyHeader() 
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

// CORRIGIDO: Chamando exatamente o nome da política que foi configurada acima
app.UseCors("SodumPolicy"); 

app.UseAuthorization();

app.MapControllers();

app.Run();
