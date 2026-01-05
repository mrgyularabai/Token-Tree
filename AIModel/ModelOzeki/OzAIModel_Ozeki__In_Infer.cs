
using System.Collections.Generic;
using System.Text;

namespace Ozeki
{

    public partial class OzAIModel_Ozeki 
    {
        public bool infer(List<int> inputTokens, out List<int> outputTokens, out string errorMessage)
        {
            outputTokens = null;

            if (!OzAIIntVec.Create(Architecture.Mode, out var ints, out errorMessage))
                return false;
            var bytes = new byte[inputTokens.Count * 4];
            var intArray = inputTokens.ToArray();
            Buffer.BlockCopy(intArray, 0, bytes, 0, intArray.Length * 4);
            if (!ints.Init(bytes, 0, (ulong)inputTokens.Count, out errorMessage))
                return false;
            Architecture.IN = ints;

            if (!Architecture.Forward(out errorMessage))
                return false;

            if (!Architecture.OUT.ToBytes(out var bytesRes, out errorMessage))
                return false;
            var resInts = new int[bytesRes.Length / 4];
            Buffer.BlockCopy(bytesRes, 0, resInts, 0, bytesRes.Length);
            outputTokens = new List<int>(resInts);

            errorMessage = null;
            return true;
        }
    }
}
