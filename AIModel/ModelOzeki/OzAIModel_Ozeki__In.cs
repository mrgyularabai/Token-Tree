
using System.Text;

namespace Ozeki
{

    public partial class OzAIModel_Ozeki 
    {
        public bool In(OzMessage input, OzChatHistory chatHistory, out OzMessage output, out string errorMessage)
        {
            return GenerateText(input, chatHistory, out output, out errorMessage);
        }

        protected bool GenerateText(OzMessage receivedMessage, OzChatHistory receivedChatHistory, out OzMessage replyMessage, out string errorMessage)
        {
            //Input text
            var inputBuilder = new StringBuilder();
            if (receivedChatHistory.Messages.Count == 0)
            {
                inputBuilder.Append(receivedMessage.Text);
            } 
            else
            {
                receivedChatHistory.AddMessage(OzChatAuthorRole.User, receivedMessage.Text);
                var chatHistoryText = receivedChatHistory.DialogToString();
                inputBuilder.Append(chatHistoryText);
            }

            var replyBuilder = new StringBuilder();
            var responseText = "";

            //Anti prompts
            var terminatorWords = AntiPrompts.Split(';');
            bool stopRequest = false;

            //Reply token limit
            var max = ReplyTokenLimit;

            //Time limit
            var timeLimitInSec = 3600;
            var stopTimeStamp = DateTime.Now.AddSeconds(timeLimitInSec);

            //Normal response end
            var responseComplete = false;

            //Build response
            while (
                !responseComplete &&
                max-- > 0 && 
                !stopRequest && 
                stopTimeStamp > DateTime.Now)
            {
                var inputText = inputBuilder.ToString();    
                if (!GetNextWord(inputText, out var nextWord, out responseComplete, out var error))
                {
                    errorMessage = "Could not generate next word. " + error;
                    replyMessage = new OzMessage(errorMessage);
                    return false;
                }

                inputBuilder.Append(nextWord);
                replyBuilder.Append(nextWord);
                responseText = replyBuilder.ToString();

                if (isAntoPrompt(ref responseText, terminatorWords)) responseComplete = true;
            }

            errorMessage = null;
            replyMessage = new OzMessage(responseText);
            return true;
        }


        bool GetNextWord(string inputText, out string outputWord, out bool responseComplete, out string errorMessage)
        {
            responseComplete = false;
            try
            {
                // Tokenization
                if (!Tokenizer.GetTokens(inputText, out var inputTokens, out var times, out var error))
                {
                    outputWord = null;
                    errorMessage = "Could not tokenize. " + error;
                    return false;
                }

                //OzLogger.Log(LogSource, LogLevel.Information, "Tokenization complete. " + times + " tok/sec");

                // Inference
                if (!infer(inputTokens, out var outputTokens, out var errorInfer))
                {
                    outputWord = null;
                    errorMessage = "Could not tokenize. " + error;
                    return false;
                }

                outputWord = Tokenizer.GetStringsRaw(outputTokens);

                //OzLogger.Log(LogSource, LogLevel.Information, "Word generated: " + outputWord);

                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Inference error. " + ex.Message;
                outputWord = null;
                return false;
            }
        }

        bool isAntoPrompt(ref string responseText, string[] terminatorwords)
        {
            foreach (var terminatorWord in terminatorwords)
            {
                if (!responseText.EndsWith(terminatorWord)) continue;
                responseText = responseText.Substring(0, responseText.Length - terminatorWord.Length);
                return true;
            }
            return false;
        }
    }
}
