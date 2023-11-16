// todo: Test FlushMode behavior (including multi-task execution)
using Xunit;
using NHibernate.Criterion;
using Microsoft.Extensions.Options;

namespace Infraestructure.Data.IntegrationTests.Data;

public class RepositoryTests : IClassFixture<IntegrationTestsFixture> 
{
    private readonly IntegrationTestsFixture _factory;

    public RepositoryTests(IntegrationTestsFixture factory)
    {
        this._factory = factory;
    }

    [Fact()] public void IRepositoryShouldBeResolved()
    { 
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var repository = sp.GetService<IRepository<FakeRecord>>();
        Assert.NotNull(repository);
    }

    [Fact()]
    public async Task RecordShouldBePersisted()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            var record = new FakeRecord() { StringProp1 = Guid.NewGuid().ToString() };
            await repository.CreateAsync(record);
            var record2 = await repository.GetAsync(x => x.StringProp1 == record.StringProp1);
            Assert.Equal(record.StringProp1, record2?.StringProp1);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task RecordShouldUpdated()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            var record = new FakeRecord() { StringProp1 = Guid.NewGuid().ToString() };
            await repository.CreateAsync(record);

            var record2 = repository.Get(record.Id);
            record2!.StringProp1 = Guid.NewGuid().ToString();
            await repository.UpdateAsync(record2);

            var record3 = repository.Get(record.Id);
            Assert.Equal(record2.StringProp1, record3!.StringProp1);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task RecordShouldBeDeleted()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            var record = new FakeRecord() { StringProp1 = Guid.NewGuid().ToString() };
            await repository.CreateAsync(record);

            var record2 = await repository.GetAsync(record.Id);
            await repository.DeleteAsync(record2);

            var record3 = await repository.GetAsync(record.Id);
            Assert.Null(record3);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact]
    public async Task ListShouldGetCorrectRecordCountForPagging()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var recordCount = 10;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) await repository.CreateAsync(new FakeRecord() { StringProp1 = $"PROP_{i}", StringProp2 = "" });
            var pagging = new Pagging() { PageSize = 5 };
            repository.List(pagging);
            Assert.Equal(recordCount, pagging.Total);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task ListShouldGetCorrectRecordCountWhenPredicateIsProvided()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var recordCount = 10;
        var expectedCount = 5;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) await repository.CreateAsync(new FakeRecord() { StringProp1 = $"PROP_{i%2}", StringProp2 = "" });
            var pagging = new Pagging() { PageSize = 5 };
            repository.List(record => record.StringProp1 == "PROP_0", pagging);
            Assert.Equal(expectedCount, pagging.Total);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }
 
    [Fact()]
    public async Task ListShouldGetCorrectRecordCountWhenCriterionIsProvided()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var recordCount = 10;
        var expectedCount = 5;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) await repository.CreateAsync(new FakeRecord() { StringProp1 = $"PROP_{i%2}", StringProp2 = "" });
            var criterion = Restrictions.Eq(nameof(FakeRecord.StringProp1), "PROP_0");
            var pagging = new Pagging() { PageSize = 5 };
            repository.List(criterion, pagging);
            Assert.Equal(expectedCount, pagging.Total);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task ListShouldReturnPaggedRecordsWhenPaggingIsProvided()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var recordCount = 10;
        var expectedRecords = 3;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) await repository.CreateAsync(new FakeRecord() { StringProp1 = $"PROP_{i%2}", StringProp2 = "" });
            var pagging = new Pagging() { PageSize = 3 };
            var records = repository.List(pagging);
            Assert.Equal(expectedRecords, records.Count());
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Theory()]
    [InlineData(10, 3, 1, 9, 7, "desc")]
    [InlineData(10, 3, 2, 6, 4, "desc")]
    [InlineData(10, 3, 4, 0, 0, "desc")]
    [InlineData(10, 3, 1, 0, 2, "asc")]
    [InlineData(10, 3, 2, 3, 5, "asc")]
    [InlineData(10, 3, 4, 9, 9, "asc")]
    public async Task ListShouldReturnOrderedPaggedRecordsWhenOrderIsProvided(int recordCount, int pageSize, int page, 
        int expectedFirstIntProp, int expectedLastIntProp, string sortDirection)
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var sut = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) { 
                await sut.CreateAsync(new FakeRecord() { IntProp = i});
            }
            var pagging = new Pagging() { 
                PageSize = pageSize, Page = page, 
                SortColumn = nameof(FakeRecord.IntProp), SortDirection = sortDirection };
            var records = sut.List(pagging);
            Assert.Equal(expectedFirstIntProp, records?.FirstOrDefault()?.IntProp);
            Assert.Equal(expectedLastIntProp, records?.LastOrDefault()?.IntProp);

        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Theory()]
    [InlineData(10, 3, 2, 3)]
    [InlineData(0, 3, 2, 0)]
    [InlineData(10, 3, 5, 0)]
    [InlineData(10, 3, 4, 1)]
    public async Task ListShouldReturnRecordsFromCorrectPageWhenPaggingIsProvided(int recordCount, int pageSize, int expectedPage, int expectedRowCount)
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) {
                var record = new FakeRecord() { StringProp1 = $"{i}", StringProp2 = GetPageForRecordId(i, pageSize).ToString() };
                await repository.CreateAsync(record);
            }
            var pagging = new Pagging() { PageSize = pageSize, Page = expectedPage };
            var records = repository.List(pagging);
            var recordsAreFromCorrectPage = records.Count(x => x.StringProp2 == expectedPage.ToString()) == expectedRowCount;
            Assert.True(recordsAreFromCorrectPage);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task ListMethodShouldApplyOrderWhenDescOrderIsConfigured()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var recordCount = 10;
        var expectedFirstProp1 = (recordCount - 1).ToString();
        var sut = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = 0; i < recordCount; i++) await sut.CreateAsync(new FakeRecord() { StringProp1 = $"{i}"});
            var records = sut.List(record => true, order => order.Desc(x => x.StringProp1));
            Assert.Equal(expectedFirstProp1, records.FirstOrDefault()?.StringProp1);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task ListMethodShouldApplyOrderWhenAscOrderIsProvided()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var recordCount = 10;
        var expectedFirstProp1 = "0";
        var sut = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            for(var i = recordCount - 1; i >= 0; i--) await sut.CreateAsync(new FakeRecord() { StringProp1 = $"{i}"});
            var records = sut.List(record => true, order => order.Asc(x => x.StringProp1));
            Assert.Equal(expectedFirstProp1, records.FirstOrDefault()?.StringProp1);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    [Fact()]
    public async Task Test1()
    {
        var sp = _factory.Services.CreateScope().ServiceProvider;
        var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
        var transactionManager = sp.GetRequiredService<ITransactionManager>();
        try
        {
            transactionManager.RequireNew();
            var record = new FakeRecord() { StringProp1 = Guid.NewGuid().ToString() };
            await repository.CreateAsync(record);

            var repository2 = sp.GetRequiredService<IRepository<FakeRecord>>();
            var record2 = await repository2.GetAsync(x => x.StringProp1 == record.StringProp1);
            Assert.Equal(record.StringProp1, record2?.StringProp1);
        }
        finally
        {
            transactionManager.Cancel();
        }
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns></returns>
    //[Fact()]
    //public async Task PropertyLengthShouldBeValidated()
    //{
    //    var sp = _factory.Services.CreateScope().ServiceProvider;
    //    var repository = sp.GetRequiredService<IRepository<FakeRecord>>();
    //    var transactionManager = sp.GetRequiredService<ITransactionManager>();
    //    try
    //    {
    //        var record = new FakeRecord() { StringProp1 = new string('*', 101) };
    //        await Assert.ThrowsAsync<DataModelValidationException>(async () => await repository.CreateAsync(record));
    //    }
    //    finally
    //    {
    //        transactionManager.Cancel();
    //    }
    //}

    public static int GetPageForRecordId(int id, int pageSize)
    {
        return Math.Abs(id / pageSize) + 1;
    }
}

public class FakeRecord : Record
{
    public virtual string StringProp1 { get; set; } = string.Empty;
    public virtual string StringProp2 { get; set; } = string.Empty;
    public virtual int IntProp { get; set; } = 0;
}
    