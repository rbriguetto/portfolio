using System.Text.Json;

namespace Infraestructure.Utils.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Converte o objeto para uma string JSON
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static string ToJson(this object o)
    {
        return JsonSerializer.Serialize(o);
    }

    /// <summary>
    /// Converte o JSON para T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="o"></param>
    /// <returns></returns>
    public static T? FromJson<T>(this string o)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(o);
    }
}