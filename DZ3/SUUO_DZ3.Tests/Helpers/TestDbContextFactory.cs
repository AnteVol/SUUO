using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;

namespace SUUO_DZ3.Tests.Helpers;

public class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}