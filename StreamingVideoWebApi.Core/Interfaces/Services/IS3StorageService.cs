using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingVideoWebApi.Core.Interfaces.Services;

public interface IS3StorageService
{
    string SignRemoteFileUrl(string key);
}
