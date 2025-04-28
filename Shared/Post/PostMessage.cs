using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Post
{
    public class PostMessage : WorkflowMessage
    {
        public string? PostId { get; set; }
        public string PostName { get; set; }
        public UserDto? User { get; set; }
    }

    public class PostResultMessage : PostMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
    }
}
