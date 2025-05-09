using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Forum
{
    public class ForumStateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint RowVersion { get; set; }
        public Guid OwnerId { get; set; }
        public List<Guid>? ModeratorIds { get; set; }
    }
}
