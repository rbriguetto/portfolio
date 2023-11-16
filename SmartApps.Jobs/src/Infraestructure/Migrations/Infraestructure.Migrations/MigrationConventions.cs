using FluentMigrator.Runner.VersionTableInfo;
using Infraestructure.Data;
using Microsoft.Extensions.Options;

namespace Infraestructure.Migrations;

[VersionTableMetaData]
public class VersionTable : IVersionTableMetaData
{
    private readonly string _tablePrefix;

    public VersionTable(IOptions<ConnectionStringOptions> options)
    {
        _tablePrefix = options.Value.TablePrefix;
    }

    public string ColumnName
    {
        get { return "Version"; }
    }

    public string SchemaName
    {
        get { return ""; }
    }

    public string TableName
    {
        get { 
                
                return $"{_tablePrefix}VersionInfo";
            }
    }

    public string UniqueIndexName
    {
        get { return $"{_tablePrefix}UC_Version"; }
    }

    public virtual string AppliedOnColumnName
    {
        get { return "AppliedOn"; }
    }

    public virtual string DescriptionColumnName
    {
        get { return "Description"; }
    }

    public object ApplicationContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool OwnsSchema => throw new NotImplementedException();
}
