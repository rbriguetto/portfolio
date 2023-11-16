using System.Collections.ObjectModel;

namespace Infraestructure.Data;

public static class ReadOnlyCollectionExtensions
{
    public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
    {
        return new ReadOnlyCollection<T>(enumerable.ToList());
    }
}