using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Comment.DTO_s
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid? PostId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public uint RowVersion { get; set; }
    }
}
