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
        public bool Embeddings;
        public uint BatchSize { get; set; }

        public struct llama_batch
        {
            public int n_tokens;

            public int[] token;
            float[] embd;
            int[] pos;
            int[] n_seq_id;
            int[][] seq_id;
            sbyte[] logits;

            int all_pos_0;
            int all_pos_1;
            int all_seq_id;
        }

        public bool Embed(llama_batch batch_all, out List<OzAIMatrix> ozAITensors, out string error)
        {
            ozAITensors = new List<OzAIMatrix>();
            error = null;
            return true;
        }
    }
}
