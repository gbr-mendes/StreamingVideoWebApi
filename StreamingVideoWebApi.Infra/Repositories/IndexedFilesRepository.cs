using Microsoft.EntityFrameworkCore;
using Optional;
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

    public async Task<Option<IndexedFile>> GetIndexedFile(Guid id)
    {
        var indexedFile = await _dbContext.IndexedFiles.FirstOrDefaultAsync(indexedFile => indexedFile.Id == id);
        return indexedFile == null ? Option.None<IndexedFile>() : Option.Some(indexedFile);
    }
}
