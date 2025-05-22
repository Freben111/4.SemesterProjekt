using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Forum.DTO_s
{
    public class ForumBackupDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Timestamp]
        public uint RowVersion { get; set; }
        public Guid OwnerId { get; set; }
        public List<Guid>? ModeratorIds { get; set; }
    }
}
