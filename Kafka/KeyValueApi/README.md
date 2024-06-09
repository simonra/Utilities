# About

A demo of a simple distributed service that allows you to store and retrieve key-value pairs using a web/REST/http API.

# Running it

1. Start the docker compose in the top level directory besides this readme to initialize all the required infrastructure components, configure them, and spin up their associated supporting tools (KafkaUI) for smoother local development.
2. After the infrastructure has spun up, start the docker compose in the `Code/` directory, to start the actual service.
3. Once it's up and running, you can use the http requests in the `Code/usage.http` to check out the functionality.

# Retracing the creation steps

Creation steps in case of demo or similar

```shell
dotnet new gitignore
dotnet new editorconfig
dotnet new sln --name "KeyValueApi"
dotnet new webapi --name "WebApi" --output "WebApi"
dotnet sln add WebApi/WebApi.csproj
cd WebApi/
dotnet add package Confluent.Kafka
dotnet add package Confluent.SchemaRegistry
dotnet add package Confluent.SchemaRegistry.Serdes.Json
```
