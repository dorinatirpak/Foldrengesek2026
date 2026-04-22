using Földrengések2026.Data; 
using Microsoft.EntityFrameworkCore;

namespace Földrengések2026.Tests;

public static class TestDbFactory
{
    public static FoldrengesContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<FoldrengesContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new FoldrengesContext(options);
    }
}