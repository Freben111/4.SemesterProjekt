using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task RollbackAsync();
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable);
    }
}
