# Premiersoft Challenge
Versão: 1.0.0 - 30-09-2025

## O que é?
Este projeto foi desenvolvido como parte de um desafio técnico para a empresa **Premiersoft**.
O sistema implementa as APIs de **conta corrente** e **transferência**.

A aplicação segue princípios de desenvolvimento de mercado em **.NET**, como DDD, CQRS, Microservices e suporte a execução via **Docker Compose**.

## Documentação
A documentação de endpoints e exemplos de uso pode ser consultada diretamente via Swagger, disponível ao executar o projeto localmente.

## Requisitos de Sistema
- **.NET 8.0 SDK** ou superior
- **Docker** e **Docker Compose** instalados (para execução em container)
- SQLite (já configurado no projeto e armazenado em arquivo local)

## Executando o Projeto

### Usando Docker Compose
Para executar toda a stack via container:

```bash
docker compose up -d
```

As APIs ficam disponíveis em:
- **Conta corrente:** http://localhost:6060
- **Transferência:** http://localhost:6061

## Executando Localmente

Clone o repositório:
```bash
git clone https://github.com/brenovandall/premiersoft_challenge.git
cd premiersoft_challenge
```

Restaure dependências:
```bash
dotnet restore
```

Rode a aplicação:
```bash
dotnet run --project src/CheckingAccount.Api
```

## Licença
Este projeto é distribuído sob a licença MIT.
