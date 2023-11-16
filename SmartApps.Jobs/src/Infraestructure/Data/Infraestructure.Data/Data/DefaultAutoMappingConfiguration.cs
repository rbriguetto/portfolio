using FluentNHibernate;
using FluentNHibernate.Automapping;

namespace Infraestructure.Data;

public class RecordAutoMapping : DefaultAutomappingConfiguration
{
    public override bool ShouldMap(Type type)
    {
        return !type.IsAbstract && typeof(IRecord).IsAssignableFrom(type);
    }

    public override bool IsId(Member member)
    {
        return member.Name == "Id";
    }
}