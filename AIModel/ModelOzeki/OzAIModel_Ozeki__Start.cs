using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ozeki
{
    public partial class OzAIModel_Ozeki 
    {
        public bool PerformStart(out string error)
        {
            if (!loadModel(modelPath, out GGUFFile, out var errorMessage))
            {
                error = "Model loaded failed. " + errorMessage;
                return false;
            }

            if (!GGUFFile.LoadTensors(out error))
                return false;


            if (!InitFromFile(GGUFFile, out var errorTok))
            {
                error = "Model Initialization failed. " + errorTok;
                return false;
            }

            error = null;
            return true;
        }

        public bool InitFromFile(OzGGUFFile gguf, out string error, uint batchSize = 512)
        {
            GGUFFile = gguf;
            BatchSize = batchSize;

            if (!GGUFFile.GetMDString("general.name", out ModelName, out error))
                return false;

            // Initialize Architecture Data
            if (!OzAIArch.CreateFromFile(GGUFFile, out Architecture, out error))
            {
                error = "Could not read architectural information. " + error;
                return false;
            }

            // Initialize Tokenizer
            if (!OzAITokenizer.CreateFromFile(GGUFFile, out this.Tokenizer, out error))
            {
                error = "Failed to create the tokenizer. " + error;
                return false;
            }

            return true;
        }
    }
}
