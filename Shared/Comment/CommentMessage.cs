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
        public List<Guid> PostIds { get; set; } = new List<Guid>();
        public string? AuthorId { get; set; }
        public string? JWT { get; set; }
    }

    public class CommentResultMessage : CommentMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; } = 400;
    }
}
