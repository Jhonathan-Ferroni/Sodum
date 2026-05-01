# Estágio 1: Build (Usa o SDK pesado para compilar o código)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura os pacotes
COPY ["Sodum.API.csproj", "./"]
RUN dotnet restore "./Sodum.API.csproj"

# Copia tudo e compila em modo Release
COPY . .
RUN dotnet publish "Sodum.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Produção (Usa apenas o Runtime leve para rodar)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Define a porta padrão que o Render vai acessar
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Sodum.API.dll"]