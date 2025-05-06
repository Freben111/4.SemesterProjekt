using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.User
{
    public class UserMessage : WorkflowMessage
    {
        public string? UserId { get; set; }
        public string UserName { get; set; }
    }

    public class UserResultMessage : UserMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
    }

    public class UserLoginMessage : UserResultMessage
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
