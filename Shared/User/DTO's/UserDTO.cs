using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.User.DTO_s
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public uint RowVersion { get; set; }

    }
}
