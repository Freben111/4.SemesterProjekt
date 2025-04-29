using CommentService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentService.Infrastructure;

namespace CommentService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CommentDbContext _db;
        private DbTransaction _transaction;
        private bool _isCommitted;

        public UnitOfWork(CommentDbContext db)
        {
            _db = db;
            _isCommitted = false;
        }


        async Task IUnitOfWork.BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            if (_db.Database.CurrentTransaction == null)
            {
                IDbContextTransaction dbContextTransaction = await _db.Database.BeginTransactionAsync(isolationLevel);
                _transaction = dbContextTransaction.GetDbTransaction();
            }
        }

        async Task IUnitOfWork.CommitAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
                await _transaction.CommitAsync();
                _isCommitted = true;
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        async Task IUnitOfWork.RollbackAsync()
        {
            if (!_isCommitted && _transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
