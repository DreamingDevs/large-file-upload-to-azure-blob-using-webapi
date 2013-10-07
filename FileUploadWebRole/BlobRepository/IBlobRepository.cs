using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IBlobRepository
    {
        bool UploadBlock(string FileId, string BlockId, Stream Data);
        bool CommintBlocks(string FileId, List<string> BlockIds);
    }
}
