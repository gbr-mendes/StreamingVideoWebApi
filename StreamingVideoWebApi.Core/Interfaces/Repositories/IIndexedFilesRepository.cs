using StreamingVideoWebApi.Core.Models;

namespace StreamingVideoWebApi.Core.Interfaces.Repositories;

public interface IIndexedFilesRepository
{
    Task<IEnumerable<IndexedFile>> GetIndexedFiles();
}
