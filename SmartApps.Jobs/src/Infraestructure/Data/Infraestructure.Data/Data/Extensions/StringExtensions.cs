namespace Oficina.Bloqueios.Infraestructure.Data.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Workaraound for mock
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static string ApplyTablePrefix(this string tableName, string tablePrefix)
    {
        return $"{tablePrefix}{tableName}";
    }
}