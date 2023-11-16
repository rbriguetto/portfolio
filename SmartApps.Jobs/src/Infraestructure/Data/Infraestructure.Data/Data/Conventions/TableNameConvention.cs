using FluentNHibernate.Conventions;

namespace Infraestructure.Data;

public class TableNameConvention : IClassConvention 
{
    private readonly string _tablePrefix;

    public TableNameConvention(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)  {
        var tableName = $"`{_tablePrefix}{instance.EntityType.Name}`";
        instance.Table(tableName);
    }
}