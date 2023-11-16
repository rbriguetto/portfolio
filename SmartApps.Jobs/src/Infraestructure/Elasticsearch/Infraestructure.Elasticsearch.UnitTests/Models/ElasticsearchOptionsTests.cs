using Xunit;
using Infraestructure.Elasticsearch.Extensions;

namespace Infraestructure.Elasticsearch.UnitTests.Models;

public class ElasticsearchOptionsTests
{
    [Theory]
    [InlineData("http://elastic:123@localhost:9200/", "http://localhost:9200/", "elastic", "123")]
    [InlineData("https://elastic:123@localhost:9200", "https://localhost:9200", "elastic", "123")]
    [InlineData("https://1:2@localhost:9200", "https://localhost:9200", "1", "2")]
    [InlineData("", "", "", "")]
    public void ShouldParseValuesValidUri(string uri, string expectedHost, string expectedUsername, string expectedPassword)
    {
        var sut = new ElasticsearchOptions() { Uri = uri };
        Assert.Equal(expectedHost, sut.Host);
        Assert.Equal(expectedUsername, sut.Username);
        Assert.Equal(expectedPassword, sut.Password);
    }
}