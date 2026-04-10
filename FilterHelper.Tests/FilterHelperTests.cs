using Xunit;
using Földrengések2026.Services;

namespace FilterHelper.Tests;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public int CategoryId { get; set; }
}

public class WhereContainsTests
{
    private static IQueryable<TestEntity> GetTestData()
    {
        return new List<TestEntity>
        {
            new() { Id = 1, Name = "Budapest", City = "Pest" },
            new() { Id = 2, Name = "Debrecen", City = "Hajdú-Bihar" },
            new() { Id = 3, Name = "Szeged", City = "Csongrád" },
            new() { Id = 4, Name = "Pécs", City = "Baranya" },
            new() { Id = 5, Name = "BUDAPEST EXTRA", City = "Pest" }
        }.AsQueryable();
    }

    [Fact]
    public void WhereContains_WithMatchingValue_FiltersCorrectly()
    {
        var data = GetTestData();

        var result = data.WhereContains("budapest", p => p.Name).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Contains("BUDAPEST", item.Name.ToUpper()));
    }

    [Fact]
    public void WhereContains_IsCaseInsensitive()
    {
        var data = GetTestData();

        var resultLower = data.WhereContains("budapest", p => p.Name).ToList();
        var resultUpper = data.WhereContains("BUDAPEST", p => p.Name).ToList();
        var resultMixed = data.WhereContains("BuDaPeSt", p => p.Name).ToList();

        Assert.Equal(resultLower.Count, resultUpper.Count);
        Assert.Equal(resultLower.Count, resultMixed.Count);
    }

    [Fact]
    public void WhereContains_WithNullValue_ReturnsAllItems()
    {
        var data = GetTestData();

        var result = data.WhereContains(null, p => p.Name).ToList();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void WhereContains_WithEmptyString_ReturnsAllItems()
    {
        var data = GetTestData();

        var result = data.WhereContains("", p => p.Name).ToList();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void WhereContains_WithNoMatch_ReturnsEmpty()
    {
        var data = GetTestData();

        var result = data.WhereContains("xyz", p => p.Name).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void WhereContains_WithPartialMatch_ReturnsMatchingItems()
    {
        var data = GetTestData();

        var result = data.WhereContains("sze", p => p.Name).ToList();

        Assert.Single(result);
        Assert.Equal("Szeged", result[0].Name);
    }
}

public class WhereEqualsTests
{
    private static IQueryable<TestEntity> GetTestData()
    {
        return new List<TestEntity>
        {
            new() { Id = 1, Date = new DateTime(2025, 1, 1), CategoryId = 10 },
            new() { Id = 2, Date = new DateTime(2025, 1, 2), CategoryId = 20 },
            new() { Id = 3, Date = new DateTime(2025, 1, 1), CategoryId = 10 },
            new() { Id = 4, Date = new DateTime(2025, 2, 1), CategoryId = 30 }
        }.AsQueryable();
    }

    [Fact]
    public void WhereEquals_WithMatchingDateTime_FiltersCorrectly()
    {
        var data = GetTestData();
        DateTime? filterDate = new DateTime(2025, 1, 1);

        var result = data.WhereEquals(filterDate, e => e.Date).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Equal(new DateTime(2025, 1, 1), item.Date));
    }

    [Fact]
    public void WhereEquals_WithMatchingInt_FiltersCorrectly()
    {
        var data = GetTestData();
        int? categoryId = 10;

        var result = data.WhereEquals(categoryId, e => e.CategoryId).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Equal(10, item.CategoryId));
    }

    [Fact]
    public void WhereEquals_WithNullValue_ReturnsAllItems()
    {
        var data = GetTestData();
        DateTime? filterDate = null;

        var result = data.WhereEquals(filterDate, e => e.Date).ToList();

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void WhereEquals_WithNoMatch_ReturnsEmpty()
    {
        var data = GetTestData();
        int? categoryId = 999;

        var result = data.WhereEquals(categoryId, e => e.CategoryId).ToList();

        Assert.Empty(result);
    }
}

public class WhereInRangeTests
{
    private static IQueryable<TestEntity> GetTestData()
    {
        return new List<TestEntity>
        {
            new() { Id = 1, Value = 1.0 },
            new() { Id = 2, Value = 2.5 },
            new() { Id = 3, Value = 3.0 },
            new() { Id = 4, Value = 5.5 },
            new() { Id = 5, Value = 8.0 }
        }.AsQueryable();
    }

    [Fact]
    public void WhereInRange_WithMinOnly_FiltersCorrectly()
    {
        var data = GetTestData();

        var result = data.WhereInRange(3.0, null, e => e.Value).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, item => Assert.True(item.Value >= 3.0));
    }

    [Fact]
    public void WhereInRange_WithMaxOnly_FiltersCorrectly()
    {
        var data = GetTestData();

        var result = data.WhereInRange(null, 3.0, e => e.Value).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, item => Assert.True(item.Value <= 3.0));
    }

    [Fact]
    public void WhereInRange_WithMinAndMax_FiltersCorrectly()
    {
        var data = GetTestData();

        var result = data.WhereInRange(2.0, 5.0, e => e.Value).ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, item =>
        {
            Assert.True(item.Value >= 2.0);
            Assert.True(item.Value <= 5.0);
        });
    }

    [Fact]
    public void WhereInRange_WithBothNull_ReturnsAllItems()
    {
        var data = GetTestData();

        var result = data.WhereInRange(null, null, e => e.Value).ToList();

        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void WhereInRange_WithExactBoundaries_IncludesBoundaryValues()
    {
        var data = GetTestData();

        var result = data.WhereInRange(2.5, 5.5, e => e.Value).ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, item => item.Value == 2.5);
        Assert.Contains(result, item => item.Value == 5.5);
    }

    [Fact]
    public void WhereInRange_WithNoItemsInRange_ReturnsEmpty()
    {
        var data = GetTestData();

        var result = data.WhereInRange(9.0, 10.0, e => e.Value).ToList();

        Assert.Empty(result);
    }
}

public class ApplySortingTests
{
    private static IQueryable<TestEntity> GetTestData()
    {
        return new List<TestEntity>
        {
            new() { Id = 1, Name = "Charlie", Value = 3.0 },
            new() { Id = 2, Name = "Alice", Value = 1.0 },
            new() { Id = 3, Name = "Bob", Value = 2.0 },
            new() { Id = 4, Name = "David", Value = 4.0 }
        }.AsQueryable();
    }

    private static Dictionary<string, SortOption<TestEntity>> GetSortOptions()
    {
        return new Dictionary<string, SortOption<TestEntity>>
        {
            ["name"] = new SortOption<TestEntity>(
                q => q.OrderBy(e => e.Name),
                q => q.OrderByDescending(e => e.Name)),
            ["value"] = new SortOption<TestEntity>(
                q => q.OrderBy(e => e.Value),
                q => q.OrderByDescending(e => e.Value))
        };
    }

    [Fact]
    public void ApplySorting_WithAscendingName_SortsCorrectly()
    {
        var data = GetTestData();
        var sortOptions = GetSortOptions();

        var result = data.ApplySorting("name", "asc", sortOptions,
            q => q.OrderBy(e => e.Name)).ToList();

        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal("Charlie", result[2].Name);
        Assert.Equal("David", result[3].Name);
    }

    [Fact]
    public void ApplySorting_WithDescendingName_SortsCorrectly()
    {
        var data = GetTestData();
        var sortOptions = GetSortOptions();

        var result = data.ApplySorting("name", "desc", sortOptions,
            q => q.OrderBy(e => e.Name)).ToList();

        Assert.Equal("David", result[0].Name);
        Assert.Equal("Charlie", result[1].Name);
        Assert.Equal("Bob", result[2].Name);
        Assert.Equal("Alice", result[3].Name);
    }

    [Fact]
    public void ApplySorting_WithAscendingValue_SortsCorrectly()
    {
        var data = GetTestData();
        var sortOptions = GetSortOptions();

        var result = data.ApplySorting("value", "asc", sortOptions,
            q => q.OrderBy(e => e.Name)).ToList();

        Assert.Equal(1.0, result[0].Value);
        Assert.Equal(2.0, result[1].Value);
        Assert.Equal(3.0, result[2].Value);
        Assert.Equal(4.0, result[3].Value);
    }

    [Fact]
    public void ApplySorting_WithDescendingValue_SortsCorrectly()
    {
        var data = GetTestData();
        var sortOptions = GetSortOptions();

        var result = data.ApplySorting("value", "desc", sortOptions,
            q => q.OrderBy(e => e.Name)).ToList();

        Assert.Equal(4.0, result[0].Value);
        Assert.Equal(3.0, result[1].Value);
        Assert.Equal(2.0, result[2].Value);
        Assert.Equal(1.0, result[3].Value);
    }

    [Fact]
    public void ApplySorting_WithUnknownSortKey_UsesDefaultSort()
    {
        var data = GetTestData();
        var sortOptions = GetSortOptions();

        var result = data.ApplySorting("unknown", "asc", sortOptions,
            q => q.OrderBy(e => e.Name)).ToList();

        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal("Charlie", result[2].Name);
        Assert.Equal("David", result[3].Name);
    }

    [Fact]
    public void ApplySorting_WithUnknownDirection_DefaultsToAscending()
    {
        var data = GetTestData();
        var sortOptions = GetSortOptions();

        var result = data.ApplySorting("name", "invalid", sortOptions,
            q => q.OrderBy(e => e.Name)).ToList();

        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
    }
}

public class CombinedFilterTests
{
    private static IQueryable<TestEntity> GetTestData()
    {
        return new List<TestEntity>
        {
            new() { Id = 1, Name = "Budapest", City = "Pest", Value = 3.5, Date = new DateTime(2025, 1, 1), CategoryId = 10 },
            new() { Id = 2, Name = "Budapest II", City = "Pest", Value = 5.0, Date = new DateTime(2025, 1, 2), CategoryId = 10 },
            new() { Id = 3, Name = "Debrecen", City = "Hajdú-Bihar", Value = 2.0, Date = new DateTime(2025, 1, 1), CategoryId = 20 },
            new() { Id = 4, Name = "Szeged", City = "Csongrád", Value = 4.0, Date = new DateTime(2025, 2, 1), CategoryId = 30 },
            new() { Id = 5, Name = "Pécs", City = "Baranya", Value = 6.0, Date = new DateTime(2025, 1, 1), CategoryId = 10 }
        }.AsQueryable();
    }

    [Fact]
    public void CombinedFilters_StringContainsAndRange_WorkTogether()
    {
        var data = GetTestData();

        var result = data
            .WhereContains("budapest", e => e.Name)
            .WhereInRange(4.0, null, e => e.Value)
            .ToList();

        Assert.Single(result);
        Assert.Equal("Budapest II", result[0].Name);
    }

    [Fact]
    public void CombinedFilters_EqualsAndRange_WorkTogether()
    {
        var data = GetTestData();
        DateTime? date = new DateTime(2025, 1, 1);

        var result = data
            .WhereEquals(date, e => e.Date)
            .WhereInRange(3.0, null, e => e.Value)
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Name == "Budapest");
        Assert.Contains(result, item => item.Name == "Pécs");
    }

    [Fact]
    public void CombinedFilters_MultipleStringContains_WorkTogether()
    {
        var data = GetTestData();

        var result = data
            .WhereContains("budapest", e => e.Name)
            .WhereContains("pest", e => e.City)
            .ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void CombinedFilters_WithSorting_WorkTogether()
    {
        var sortOptions = new Dictionary<string, SortOption<TestEntity>>
        {
            ["value"] = new SortOption<TestEntity>(
                q => q.OrderBy(e => e.Value),
                q => q.OrderByDescending(e => e.Value))
        };

        var data = GetTestData();
        int? categoryId = 10;

        var result = data
            .WhereEquals(categoryId, e => e.CategoryId)
            .ApplySorting("value", "asc", sortOptions, q => q.OrderBy(e => e.Value))
            .ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(3.5, result[0].Value);
        Assert.Equal(5.0, result[1].Value);
        Assert.Equal(6.0, result[2].Value);
    }
}
