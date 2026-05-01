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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
