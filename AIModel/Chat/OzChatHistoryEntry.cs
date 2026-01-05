using Ozeki;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIModel.Chat
{
    public class OzChatHistoryEntry
    {
        public OzChatAuthorRole Role;
        public OzMessage Message;

        public OzChatHistoryEntry(OzChatAuthorRole role, OzMessage message)
        { 
            Role = role;
            Message = message;
        }

        public override string ToString()
        {
            var ret = new StringBuilder();
            switch (Role)
            {
                case OzChatAuthorRole.System:
                    ret.Append("System: ");
                    break;
                case OzChatAuthorRole.User:
                    ret.Append("User: ");
                    break;
                case OzChatAuthorRole.Assistant:
                    ret.Append("Assistant: ");
                    break;
            }
            ret.Append(Message.Text);
            return ret.ToString();
        }
    }
}
