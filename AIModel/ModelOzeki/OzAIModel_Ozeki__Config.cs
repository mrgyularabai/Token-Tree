using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ozeki
{
    partial class OzAIModel_Ozeki
    {


        //***************************************************
        // Model path
        //***************************************************

        public string modelPath
        {
            get;
            set;
        }

        //***************************************************
        // Model Help
        //***************************************************

        public string ModelHelp
        {
            get { return String.Empty; }
        }

        //***************************************************
        // Session settings
        //***************************************************

        public string AntiPrompts
        {
            get;
            set;
        }

        public bool UseTemperature
        {
            get;
            set;
        }

        public float Temperature
        {
            get;
            set;
        }


        //***************************************************
        // HW settings
        //***************************************************

        public int GPULayers
        {
            get;
            set;
        }


        public int ContextWindowSize
        {
            get;
            set;
        }

        public int ReplyTokenLimit
        {
            get;
            set;
        }

        public int RepeatLastTokenCount
        {
            get;
            set;
        }

        //***************************************************
        // Default values
        //***************************************************

        public Dictionary<string, string> GetModelDescription()
        {
            var ret = new Dictionary<string, string>();
            ret.Add("Model", modelPath);
            ret.Add("ContextWindowSize", ContextWindowSize.ToString());
            ret.Add("GPULayers", GPULayers.ToString());
            ret.Add("ReplyTokenLimit", ReplyTokenLimit.ToString());
            ret.Add("Temperature", Temperature.ToString());
            return ret;
        }

        public void SetModelDescription(Dictionary<string, string> description)
        {
            if (description == null) return;

            if (description.ContainsKey("Model"))
            {
                modelPath = description["Model"];
            }

            if (description.ContainsKey("ContextWindowSize"))
            {
                var cws = description["ContextWindowSize"];
                if (int.TryParse(cws, out var i))
                {
                    ContextWindowSize = i;
                }
            }

            if (description.ContainsKey("GPULayers"))
            {
                var cws = description["GPULayers"];
                if (int.TryParse(cws, out var i))
                {
                    GPULayers = i;
                }
            }

            if (description.ContainsKey("ReplyTokenLimit"))
            {
                var cws = description["ReplyTokenLimit"];
                if (int.TryParse(cws, out var i))
                {
                    ReplyTokenLimit = i;
                }
            }

            if (description.ContainsKey("Temperature"))
            {
                var cws = description["Temperature"];
                if (float.TryParse(cws, out var f))
                {
                    Temperature = f;
                    UseTemperature = true;
                }
            }

            if (description.ContainsKey("RepeatLastTokensCount"))
            {
                var cws = description["RepeatLastTokensCount"];
                if (int.TryParse(cws, out var i))
                {
                    RepeatLastTokenCount = i;
                    UseTemperature = true;
                }
            }

            if (description.ContainsKey("AntiPrompts"))
            {
                var aps = description["AntiPrompts"];
                AntiPrompts = aps;
            }
        }

        //***************************************************
        // Default values
        //***************************************************

        public void InitLoadedOrCreated()
        {
            modelPath = "C:\\AIModels\\Meta-Llama-3.1-8B-Instruct-Q4_K_M.gguf";

            ContextWindowSize = 128000;
            GPULayers = 130;
            ReplyTokenLimit = 10000;
            Temperature = 0.3f;
            RepeatLastTokenCount = 0;
            AntiPrompts = "User:";
        }
    }
}
