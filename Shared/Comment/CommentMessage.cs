using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Comment
{
    public class CommentMessage : WorkflowMessage
    {
        public string? CommentId { get; set; }
        public UserDto? User { get; set; }
    }

    public class CommentResultMessage : CommentMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
    }
}
