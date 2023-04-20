# WebAPI

## Install NUKE

```
dotnet tool install Nuke.GlobalTool --global
```

## Start development environment

```
nuke StartEnv
```

- **Seq**: http://localhost:5342/
- **RabbitMQ**: http://localhost:15671/
  - User: admin
  - Password: Rabbitmq123$
- **SQLServer**: localhost,1033
  - User: sa
  - Password: Sqlserver123$

## Update database schema

```
nuke RunMigrator
```

## Run integration tests

```
nuke Test
```


