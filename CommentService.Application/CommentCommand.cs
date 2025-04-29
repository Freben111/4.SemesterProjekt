using Dapr.Client;
using CommentService.Application.Interfaces;
using CommentService.Domain;
using CommentService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using CommentService.Application.Interfaces;
using Shared.Comment;
using Shared.Comment.DTO_s;

namespace CommentService.Application
{
    public class CommentCommand : ICommentCommand
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<CommentCommand> _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CommentCommand(DaprClient daprClient, ILogger<CommentCommand> logger, ICommentRepository CommentRepository, IUnitOfWork unitOfWork)
        {
            _daprClient = daprClient;
            _logger = logger;
            _commentRepository = CommentRepository;
            _unitOfWork = unitOfWork;
        }

        async Task<CommentResultMessage> ICommentCommand.CreateComment(CreateCommentDTO dto)
        {
            var result = new CommentResultMessage
            {
                Status = "Creating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var comment = Comment.CreateComment(dto.Content, dto.PostId, dto.UserId);

                await _commentRepository.CreateComment(comment);
                await _unitOfWork.CommitAsync();

                await _daprClient.SaveStateAsync("statestore", comment.Id.ToString(), comment);

                _logger.LogInformation("Comment for post {PostId} created successfully", dto.PostId);
                result.CommentId = comment.Id.ToString();
                result.Status = "Created";
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating comment");
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }


        async Task<CommentResultMessage> ICommentCommand.UpdateComment(Guid commentId, UpdateCommentDTO dto)
        {
            var result = new CommentResultMessage
            {
                CommentId = commentId.ToString(),
                Status = "Updating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var comment = await _commentRepository.GetCommentById(commentId);
                if (comment == null)
                {
                    throw new Exception("Comment not found");
                }
                comment.UpdateComment(dto.Content);
                await _commentRepository.UpdateComment(comment, comment.RowVersion);
                await _unitOfWork.CommitAsync();

                await _daprClient.SaveStateAsync("statestore", comment.Id.ToString(), comment);

                result.Status = "Updated";
                _logger.LogInformation("Comment {CommentId} updated successfully", commentId);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error updating Comment {CommentId}", commentId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

        async Task<CommentResultMessage> ICommentCommand.DeleteComment(Guid commentId)
        {
            var result = new CommentResultMessage
            {
                CommentId = commentId.ToString(),
                Status = "Deleting"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var comment = await _commentRepository.GetCommentById(commentId);
                if (comment == null)
                {
                    throw new Exception("Comment not found");
                }

                await _commentRepository.DeleteComment(comment, comment.RowVersion);
                await _unitOfWork.CommitAsync();

                var cachedComment = await _daprClient.GetStateAsync<Comment>("statestore", commentId.ToString());
                if (cachedComment != null)
                {
                    await _daprClient.DeleteStateAsync("statestore", commentId.ToString());
                }

                _logger.LogInformation("Comment {CommentId} deleted successfully", commentId);
                result.Status = "Deleted";
                return result;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

    }

}
