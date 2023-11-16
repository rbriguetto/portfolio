using FluentMigrator;
using Microsoft.Extensions.Options;
using Infraestructure.Data;

namespace SmartApps.Jobs.Infraestructure.Data;

[Migration(100000001, "Criação das tabelas básicas do Gateway:Jobs")]
public class Migrations00001 : Migration
{
    private readonly string _tablePrefix;

    public Migrations00001(IOptions<ConnectionStringOptions> options)
    {
        _tablePrefix = options.Value.TablePrefix;
    }

    public override void Down() { }

    public override void Up()
    {
        Create.Table($"{_tablePrefix}{nameof(JobRecord)}")
            .WithColumn(nameof(JobRecord.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(JobRecord.JobId)).AsString(50).Unique()
            .WithColumn(nameof(JobRecord.UniqueId)).AsString(100).Nullable() 
            .WithColumn(nameof(JobRecord.Handler)).AsString(100).Nullable()
            .WithColumn(nameof(JobRecord.Payload)).AsString(10000).Nullable()
            .WithColumn(nameof(JobRecord.CreateUtc)).AsDateTime()
            .WithColumn(nameof(JobRecord.StartedAtUtc)).AsDateTime().Nullable()
            .WithColumn(nameof(JobRecord.FinishedAtUtc)).AsDateTime().Nullable()
            .WithColumn(nameof(JobRecord.Response)).AsString(10000).Nullable()
            .WithColumn(nameof(JobRecord.UserAgent)).AsString(500).Nullable()
            .WithColumn(nameof(JobRecord.Status)).AsString(20).Nullable();
    }
}