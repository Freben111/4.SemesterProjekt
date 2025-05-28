using Shared.Post.DTO_s;
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
        public string? PostName { get; set; }
        public string? AuthorId { get; set; }
        public string? JWT { get; set; }
        public Guid? ForumId { get; set; }
    }

    public class PostResultMessage : PostMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; } = 400;

        public List<PostBackupDTO?> BackupPost { get; set; } = new List<PostBackupDTO?>();
    }
}
