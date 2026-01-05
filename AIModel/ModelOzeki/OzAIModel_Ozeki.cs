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
        public string ModelName;
        public OzGGUFFile GGUFFile;
        public OzAIArch Architecture;
        public OzAITokenizer Tokenizer;
        //public OzAIEmbedding Embedding;

        protected string GetDescription()
        {
            if (!File.Exists(modelPath)) return "Model file not found";
            return Path.GetFileName(modelPath); 
        }
    }
}
