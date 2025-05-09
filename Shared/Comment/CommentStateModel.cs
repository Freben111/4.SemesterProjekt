using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Comment
{
    public class CommentStateModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid? PostId { get; set; }
        public Guid? AuthorId { get; set; }
        public string? UserName { get; set; }
    }
}
