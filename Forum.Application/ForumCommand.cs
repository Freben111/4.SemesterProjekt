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
        public ForumCommand(DaprClient daprClient, ILogger<ForumCommand> logger,  IForumRepository forumRepository, IUnitOfWork unitOfWork)
        {
            _daprClient = daprClient;
            _logger = logger;
            _forumRepository = forumRepository;
            _unitOfWork = unitOfWork;
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

                var existingForum = _daprClient.GetStateAsync<ForumResultMessage>("statestore", dto.Name).Result;
                if (existingForum != null)
                {
                    throw new Exception("Forum already exists");
                }

                var forum = Forum.CreateForum(dto.Name, dto.Description, userId);

                await _forumRepository.CreateForum(forum);
                await _unitOfWork.CommitAsync();

                await _daprClient.SaveStateAsync("statestore", forum.Id.ToString(), forum);

                _logger.LogInformation("Forum {ForumName} created successfully", dto.Name);
                result.ForumId = forum.Id.ToString();
                result.Status = "Created";
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
                    throw new Exception("Forum not found");
                }
                if (forum.OwnerId != userId || !forum.ModeratorIds.Contains(userId))
                {
                    throw new Exception("User not authorized to update this forum");
                }
                forum.UpdateForum(dto.Name, dto.Description);
                await _forumRepository.UpdateForum(forum, forum.RowVersion);
                await _unitOfWork.CommitAsync();

                await _daprClient.SaveStateAsync("statestore", forum.Name, forum);

                result.Status = "Updated";
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
                    throw new Exception("Forum not found");
                }
                if (forum.OwnerId != userId)
                {
                    throw new Exception("User not authorized to delete this forum");
                }

                await _forumRepository.DeleteForum(forum, forum.RowVersion);
                await _unitOfWork.CommitAsync();

                var cachedForum = await _daprClient.GetStateAsync<Forum>("statestore", forumId.ToString());
                if (cachedForum != null)
                {
                    await _daprClient.DeleteStateAsync("statestore", forumId.ToString());
                }

                _logger.LogInformation("Forum {ForumId} deleted successfully", forumId);
                result.Status = "Deleted";
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

    }

}
