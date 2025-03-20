using Microsoft.EntityFrameworkCore;
using StreamingVideoWebApi.Core.Models;

namespace StreamingVideoIndexer.Infra.DatabaseContext;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<IndexedFile> IndexedFiles { get; set; }
}
