# Sodum

Projeto fullstack com API e Frontend para o site https://sodum.vercel.app/. O objetivo do projeto é fornecer uma aplicação moderna, responsiva e com backend em C# capaz de atender a interface web.

## Visão geral

- Frontend: interface web disponível em https://sodum.vercel.app/
- API: backend em C# para operações e dados do sistema
- Arquitetura: separação clara entre API e Frontend para facilitar manutenção e evolução

## Funcionalidades

- Autenticação e autorização de usuários
- Consumo de dados pela interface web
- Layout responsivo para desktop e mobile
- Integração entre front e back com chamadas REST

## Tecnologias

- Backend: C# (.NET)
- Frontend: HTML, CSS, JavaScript/React ou framework equivalente
- API RESTful
- Deploy: Vercel para frontend

## Screenshots

![Screenshot 1](media/1.png)
![Screenshot 2](media/2.png)
![Screenshot 3](media/3.png)

## Estrutura do projeto

- `api/` - código do backend em C# (.NET)
- `front/` - código da aplicação web
- `README.md` - documentação do projeto

## Como rodar localmente

1. Clone o repositório:

   ```bash
   git clone https://github.com/seu-usuario/sodum.git
   ```

2. Abra a pasta do backend e restaure dependências:

   ```bash
   cd sodum/api
   dotnet restore
   dotnet build
   dotnet run
   ```

3. Abra a pasta do frontend e instale dependências:

   ```bash
   cd ../front
   npm install
   npm run dev
   ```

4. Acesse a aplicação no navegador em `http://localhost:3000` ou na porta configurada.

## Implantação

- Frontend implantado em Vercel: https://sodum.vercel.app/
- Backend pode ser publicado em Azure, AWS, Heroku ou outro serviço compatível com .NET

## Contato

Para dúvidas ou sugestões, entre em contato com o responsável pelo projeto.
