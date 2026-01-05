using AIModel.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozeki
{
    public class OzChatHistory
    {
        public List<OzChatHistoryEntry> Messages;

        public OzChatHistory()
        { 
            Messages = new List<OzChatHistoryEntry>();
        }

        public void AddMessage(OzChatAuthorRole role, string text)
        {
            var m = new OzMessage(text);
            Messages.Add(new OzChatHistoryEntry(role, m));
        }
        public string DialogToString()
        { 
            var ret = new StringBuilder();
            foreach (var m in Messages) {
                ret.Append(m.ToString()); 
            }
            return ret.ToString();
        }
    }
}
