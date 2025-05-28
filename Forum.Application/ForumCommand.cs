using Dapr.Client;
using ForumService.Application.Interfaces;
using ForumService.Domain;
using ForumService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Forum;
using Shared.Forum.DTO_s;

namespace ForumService.Application
{
    public class ForumCommand : IForumCommand
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<ForumCommand> _logger;
        private readonly IForumRepository _forumRepository; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IForumStateModelCommand _forumState;
        public ForumCommand(DaprClient daprClient, ILogger<ForumCommand> logger,  IForumRepository forumRepository, IUnitOfWork unitOfWork, IForumStateModelCommand forumState)
        {
            _daprClient = daprClient;
            _logger = logger;
            _forumRepository = forumRepository;
            _unitOfWork = unitOfWork;
            _forumState = forumState;
        }

        async Task<ForumResultMessage> IForumCommand.CreateForum(CreateForumDTO dto, Guid userId)
        {
            var result = new ForumResultMessage
            {
                ForumName = dto.Name,
                Status = "Creating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var forum = Forum.CreateForum(dto.Name, dto.Description, userId);

                await _forumRepository.CreateForum(forum);
                await _unitOfWork.CommitAsync();

                var forumState = _forumState.Create(forum);
                await _daprClient.SaveStateAsync("blogstatestore", forum.Id.ToString(), forumState);

                result.ForumId = forum.Id.ToString();
                result.ForumName = dto.Name;
                result.OwnerId = userId.ToString();
                result.Status = "Created";
                result.StatusCode = 201;

                _logger.LogInformation("Forum {ForumName} created successfully", dto.Name);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating forum {ForumName}", dto.Name);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }

        }


        async Task<ForumResultMessage> IForumCommand.UpdateForum(Guid forumId, UpdateForumDTO dto, Guid userId)
        {
            var result = new ForumResultMessage
            {
                ForumId = forumId.ToString(),
                Status = "Updating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var forum = await _forumRepository.GetForumById(forumId);
                if (forum == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("Forum not found");
                }
                if (forum.OwnerId != userId || !forum.ModeratorIds.Contains(userId))
                {
                    result.StatusCode = 403;
                    throw new Exception("User not authorized to update this forum");
                }
                forum.UpdateForum(dto.Name, dto.Description);
                await _forumRepository.UpdateForum(forum, forum.RowVersion);
                await _unitOfWork.CommitAsync();

                var cachedForum = _forumState.Create(forum);
                await _daprClient.SaveStateAsync("blogstatestore", forum.Id.ToString(), cachedForum);

                result.ForumId = forum.Id.ToString();
                result.ForumName = dto.Name;
                result.OwnerId = userId.ToString();
                result.Status = "Updated";
                result.StatusCode = 200;

                _logger.LogInformation("Forum {ForumId} updated successfully", forumId);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error updating forum {ForumId}", forumId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

        async Task<ForumResultMessage> IForumCommand.DeleteForum(Guid forumId, Guid userId)
        {
            var result = new ForumResultMessage
            {
                ForumId = forumId.ToString(),
                Status = "Deleting"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var forum = await _forumRepository.GetForumById(forumId);
                if (forum == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("Forum not found");
                }
                if (forum.OwnerId != userId)
                {
                    result.StatusCode = 403;
                    throw new Exception("User not authorized to delete this forum");
                }

                var backupForum = new ForumBackupDTO
                {
                    Id = forum.Id,
                    Name = forum.Name,
                    Description = forum.Description,
                    CreatedAt = forum.CreatedAt,
                    UpdatedAt = forum.UpdatedAt,
                    RowVersion = forum.RowVersion,
                    OwnerId = forum.OwnerId,
                    ModeratorIds = forum.ModeratorIds
                };

                await _forumRepository.DeleteForum(forum, forum.RowVersion);
                await _unitOfWork.CommitAsync();

                await _daprClient.DeleteStateAsync("blogstatestore", forumId.ToString());

                result.ForumId = forum.Id.ToString();
                result.ForumName = forum.Name;
                result.OwnerId = userId.ToString();
                result.Status = "Forum Deleted";
                result.StatusCode = 200;
                result.BackupForum = backupForum;

                _logger.LogInformation("Forum {ForumId} deleted successfully", forumId);
                return result;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error deleting forum {ForumId}", forumId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

        async Task<ForumResultMessage> IForumCommand.RestoreForum(ForumBackupDTO backup)
        {
            var result = new ForumResultMessage
            {
                ForumId = backup.Id.ToString(),
                Status = "Restoring"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var forum = Forum.RestoreForum(backup.Id, backup.Name, backup.Description, backup.CreatedAt, backup.UpdatedAt, backup.OwnerId, backup.ModeratorIds);
                await _forumRepository.CreateForum(forum);
                await _unitOfWork.CommitAsync();

                var cachedForum = _forumState.Create(forum);
                await _daprClient.SaveStateAsync("blogstatestore", forum.Id.ToString(), cachedForum);

                result.ForumId = forum.Id.ToString();
                result.ForumName = backup.Name;
                result.OwnerId = backup.OwnerId.ToString();
                result.Status = "Restored";
                result.StatusCode = 200;

                _logger.LogInformation("Forum {ForumId} restored successfully", backup.Id);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error restoring forum {ForumId}", backup.Id);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

    }

}
