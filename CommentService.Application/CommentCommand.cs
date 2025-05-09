using CommentService.Application.Interfaces;
using CommentService.Domain;
using CommentService.Domain.Interfaces;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Shared.Comment;
using Shared.Comment.DTO_s;
using static Google.Rpc.Context.AttributeContext.Types;

namespace CommentService.Application
{
    public class CommentCommand : ICommentCommand
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<CommentCommand> _logger;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentStateModelCommand _commentStateModel;
        public CommentCommand(DaprClient daprClient, ILogger<CommentCommand> logger, ICommentRepository CommentRepository, IUnitOfWork unitOfWork, ICommentStateModelCommand commentStateModel)
        {
            _daprClient = daprClient;
            _logger = logger;
            _commentRepository = CommentRepository;
            _unitOfWork = unitOfWork;
            _commentStateModel = commentStateModel;
        }

        async Task<CommentResultMessage> ICommentCommand.CreateComment(CreateCommentDTO dto, Guid authId)
        {
            var result = new CommentResultMessage
            {
                Status = $"Creating comment on post {dto.PostId}"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var comment = Comment.CreateComment(dto.Content, dto.PostId, authId);

                await _commentRepository.CreateComment(comment);
                await _unitOfWork.CommitAsync();

                var commentState = _commentStateModel.Create(comment);
                await _daprClient.SaveStateAsync("statestore", comment.Id.ToString(), commentState);

                result.CommentId = comment.Id.ToString();
                result.Status = "Created";
                result.PostId = dto.PostId.ToString();
                result.AuthorId = authId.ToString();
                result.StatusCode = 201;


                _logger.LogInformation("Comment for post {PostId} created successfully", dto.PostId);
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


        async Task<CommentResultMessage> ICommentCommand.UpdateComment(Guid commentId, UpdateCommentDTO dto, Guid authId)
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
                    result.StatusCode = 404;
                    throw new Exception("Comment not found");
                }
                if (comment.AuthorId != authId)
                {
                    result.StatusCode = 403;
                    throw new Exception("You are not authorized to update this user");
                }
                comment.UpdateComment(dto.Content);
                await _commentRepository.UpdateComment(comment, comment.RowVersion);
                await _unitOfWork.CommitAsync();

                var commentState = _commentStateModel.Create(comment);
                await _daprClient.SaveStateAsync("statestore", comment.Id.ToString(), comment);

                result.CommentId = comment.Id.ToString();
                result.Status = "Updated";
                result.PostId = comment.PostId.ToString();
                result.AuthorId = comment.AuthorId.ToString();
                result.StatusCode = 200;

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

        async Task<CommentResultMessage> ICommentCommand.DeleteComment(Guid commentId, Guid authId)
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
                    result.StatusCode = 404;
                    throw new Exception("Comment not found");
                }
                if (comment.AuthorId != authId)
                {
                    result.StatusCode = 403;
                    throw new Exception("You are not authorized to update this user");
                }

                await _commentRepository.DeleteComment(comment, comment.RowVersion);
                await _unitOfWork.CommitAsync();

                await _daprClient.DeleteStateAsync("statestore", commentId.ToString());

                result.CommentId = comment.Id.ToString();
                result.Status = "Deleted";
                result.PostId = comment.PostId.ToString();
                result.AuthorId = comment.AuthorId.ToString();
                result.StatusCode = 200;

                _logger.LogInformation("Comment {CommentId} deleted successfully", commentId);
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
