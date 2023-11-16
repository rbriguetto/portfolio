using Xunit;
using Infraestructure.Utils.Extensions;

namespace Infraestructure.Utils.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ShouldSerializeObject()
    {
        var input = new DummyObject();
        var serializedText = input.ToJson();
        var output = serializedText.FromJson<DummyObject>();
        Assert.Equal(input.StringProperty, output?.StringProperty);
        Assert.Equal(input.IntProperty, output?.IntProperty);
        Assert.Equal(input.DecimalProperty, output?.DecimalProperty);
        Assert.Equal(input.DateTimeProperty, output?.DateTimeProperty);
    }
}

public class DummyObject
{
    public string StringProperty { get; set; } = Guid.NewGuid().ToString();
    public int IntProperty { get; set; } = 999;
    public decimal DecimalProperty { get; set; } = 1.99m;
    public DateTime DateTimeProperty { get; set; } = DateTime.UtcNow;
}
