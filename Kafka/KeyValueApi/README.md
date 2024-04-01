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
