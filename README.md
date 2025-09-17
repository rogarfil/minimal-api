Este arquivo explica o projeto, as tecnologias usadas e o passo a passo para a execução, facilitando para qualquer pessoa que acesse e necessite da API.

-----
# rogarfil\_api

Este repositório contém uma **Minimal API** desenvolvida em C\# com .NET, seguindo os princípios de uma API mínima para fornecer endpoints de autenticação, CRUD de usuários e gerenciamento de banco de dados. O projeto foi criado como parte do desafio prático da plataforma Digital Innovation One (DIO), com o objetivo de replicar e aprimorar um projeto de destaque.

## Tecnologias Utilizadas

O projeto foi construído utilizando as seguintes tecnologias e bibliotecas:

  * **C\#** e **.NET 9.0**: Framework principal para o desenvolvimento da API.
  * **Minimal APIs**: Abordagem leve e eficiente para construir endpoints HTTP.
  * **Entity Framework Core**: ORM (Object-Relational Mapper) para manipulação do banco de dados.
  * **SQLite**: Banco de dados leve e integrado, ideal para o desenvolvimento.
  * **JWT (JSON Web Tokens)**: Usado para a autenticação e autorização de endpoints protegidos.
  * **BCrypt.Net-Next**: Biblioteca para hash de senhas de forma segura.
  * **Swagger/OpenAPI**: Geração de documentação interativa da API.

-----

## Como Executar o Projeto

Siga os passos abaixo para clonar o repositório, configurar o ambiente e executar a API em sua máquina.

### Pré-requisitos

Certifique-se de ter os seguintes softwares instalados:

  * **Git**
  * **SDK do .NET 9.0**

### 1\. Clonar o Repositório

Primeiro, clone este repositório para sua máquina local.

```bash
git clone https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git
cd SEU_REPOSITORIO
```

### 2\. Acessar a Pasta do Projeto

Navegue até o diretório do projeto `rogarfil_api`:

```bash
cd rogarfil_api
```

### 3\. Instalar Dependências

Instale os pacotes NuGet necessários para o projeto:

```bash
dotnet restore
```

### 4\. Executar as Migrações do Banco de Dados

Crie o banco de dados e as tabelas necessárias utilizando o Entity Framework Core. O banco de dados `rogarfil.db` será criado automaticamente.

```bash
dotnet ef database update
```

### 5\. Executar a Aplicação

Inicie a aplicação. O servidor será executado em `https://localhost:7000`.

```bash
dotnet run
```

Uma vez que a aplicação esteja em execução, você pode acessar a documentação interativa do Swagger em `https://localhost:7000/swagger`.

-----

## Endpoints da API

A API oferece os seguintes endpoints:

### Endpoints Públicos

  * `POST /api/login`: Endpoint para autenticação de usuários. Recebe `Username` e `Password` e retorna um JWT para ser usado em endpoints protegidos.
  * `POST /api/users`: Cria um novo usuário. A senha é automaticamente hasheada com BCrypt.
  * `POST /api/migrate`: Executa as migrações do banco de dados.

### Endpoints Protegidos (Requerem JWT)

  * `GET /api/users`: Lista todos os usuários cadastrados.
  * `GET /api/users/{id}`: Busca um usuário específico pelo ID.

-----

## Contribuindo

Contribuições são sempre bem-vindas\! Se você deseja colaborar, siga os passos abaixo:

1.  Faça um `fork` deste repositório.
2.  Crie uma nova `branch` para sua feature (`git checkout -b minha-feature`).
3.  Faça suas alterações e `commit` (`git commit -m 'feat: Minha nova feature'`).
4.  Envie suas alterações (`git push origin minha-feature`).
5.  Abra um `Pull Request` detalhando suas mudanças.
