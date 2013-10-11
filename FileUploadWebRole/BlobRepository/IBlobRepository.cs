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
        bool UploadBlock(string fileId, string blockId, Stream data);
        void CommintBlocks(string fileId, List<string> blockIds);
    }
}
