using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Comment.DTO_s
{
    public class CreateCommentDTO
    {
        public string Content { get; set; }
        public string PostId { get; set; }
    }
}
