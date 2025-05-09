using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Forum
{
    public class ForumMessage : WorkflowMessage
    {
        public string? ForumId { get; set; }
        public string ForumName{ get; set; }
        public string? OwnerId { get; set; }
    }

    public class ForumResultMessage : ForumMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; } = 400;
    }
}
