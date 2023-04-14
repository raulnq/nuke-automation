// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using Core.Migrator;
using Polly;

var migrator = new DbMigrator(typeof(Program).Assembly);

Policy
    .Handle<SqlException>(exception => exception.Message.Contains("pre-login handshake"))
    .WaitAndRetry(new[]
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    })
    .Execute(() => migrator.Migrate());