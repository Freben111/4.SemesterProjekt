using PostService.Domain;
using Shared.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostService.Application.Interfaces
{
    public interface IPostStateModelCommand
    {
        public PostStateModel Create(Post post);
    }
}
