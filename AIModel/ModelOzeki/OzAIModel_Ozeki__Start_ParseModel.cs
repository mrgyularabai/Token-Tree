using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ozeki
{

    public partial class OzAIModel_Ozeki 
    {
        bool loadModel(string path, out OzGGUFFile GGUFFile, out string error)
        {
            if (!OzGGUFFile.Create(path, out GGUFFile, out error)) return false;

            /*OzLogger.Log(LogSource, LogLevel.Information, "[GGUF] Version: " + GGUFFile.VersionNumber);
            OzLogger.Log(LogSource, LogLevel.Information, "[GGUF] Metas: " + GGUFFile.MDCount);
            OzLogger.Log(LogSource, LogLevel.Information, "[GGUF] Tensors: " + GGUFFile.TensorCount);
            OzLogger.Log(LogSource, LogLevel.Information, "[GGUF] Architecture: " + GGUFFile.Architecture);
            var meta = GGUFFile.MetaDescriptions();
            OzLogger.Log(LogSource, LogLevel.Information, "[GGUF]", meta, false);
            //var tensorDesc = GGUFFile.TensorDescriptions();
            //OzLogger.Log(LogSource, LogLevel.Information, "[GGUF]", tensorDesc, false);
            */
            return true;
        }
    }
}
