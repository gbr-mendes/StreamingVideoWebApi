using Microsoft.EntityFrameworkCore;
using StreamingVideoIndexer.Infra.DatabaseContext;
using StreamingVideoWebApi.Core.Interfaces.Repositories;
using StreamingVideoWebApi.Core.Models;

namespace StreamingVideoWebApi.Infra.Repositories;

public class IndexedFilesRepository : IIndexedFilesRepository
{
    private readonly DatabaseContext _dbContext;
    public IndexedFilesRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<IndexedFile>> GetIndexedFiles()
    {
        var indexdFiles = await _dbContext.IndexedFiles.ToListAsync();
        return indexdFiles;
    }
}
