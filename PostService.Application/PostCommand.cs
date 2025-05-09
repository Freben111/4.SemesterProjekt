using Dapr.Client;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;
using PostService.Domain;
using PostService.Domain.Interfaces;
using Shared.Post;
using Shared.Post.DTO_s;
using static Google.Rpc.Context.AttributeContext.Types;

namespace PostService.Application
{
    public class PostCommand : IPostCommand
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<PostCommand> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostStateModelCommand _postStateModel;
        public PostCommand(DaprClient daprClient, ILogger<PostCommand> logger, IPostRepository postRepository, IUnitOfWork unitOfWork, IPostStateModelCommand postStateModel)
        {
            _daprClient = daprClient;
            _logger = logger;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _postStateModel = postStateModel;
        }

        async Task<PostResultMessage> IPostCommand.CreatePost(CreatePostDTO dto, Guid userId)
        {
            var result = new PostResultMessage
            {
                PostName = dto.Title,
                Status = "Creating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var post = Post.CreatePost(dto.Title, dto.Content, dto.ForumId, userId);

                await _postRepository.CreatePost(post);
                await _unitOfWork.CommitAsync();

                var postState = _postStateModel.Create(post);
                await _daprClient.SaveStateAsync("statestore", post.Id.ToString(), postState);


                result.PostId = post.Id.ToString();
                result.PostName = dto.Title;
                result.AuthorId = userId.ToString();
                result.Status = "Created";
                result.StatusCode = 201;

                _logger.LogInformation("Post {PostName} created successfully", dto.Title);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating post {PostName}", dto.Title);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }


        async Task<PostResultMessage> IPostCommand.UpdatePost(Guid postId, UpdatePostDTO dto, Guid authId)
        {
            var result = new PostResultMessage
            {
                PostId = postId.ToString(),
                Status = "Updating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var post = await _postRepository.GetPostById(postId);
                if (post == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("Post not found");
                }
                if (post.AuthorId != authId)
                {
                    result.StatusCode = 403;
                    throw new Exception("You are not authorized to update this user");
                }
                post.UpdatePost(dto.Title, dto.Content);
                await _postRepository.UpdatePost(post, post.RowVersion);
                await _unitOfWork.CommitAsync();

                var postState = _postStateModel.Create(post);
                await _daprClient.SaveStateAsync("statestore", post.Id.ToString(), postState);

                result.PostId = post.Id.ToString();
                result.PostName = dto.Title;
                result.AuthorId = authId.ToString();
                result.Status = "Updated";
                result.StatusCode = 200;

                _logger.LogInformation("Post {PostId} updated successfully", postId);
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error updating post {PostId}", postId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

        async Task<PostResultMessage> IPostCommand.DeletePost(Guid postId, Guid authId)
        {
            var result = new PostResultMessage
            {
                PostId = postId.ToString(),
                Status = "Deleting"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var post = await _postRepository.GetPostById(postId);
                if (post == null)
                {
                    result.StatusCode = 404;
                    throw new Exception("Post not found");
                }
                if (post.AuthorId != authId)
                {
                    result.StatusCode = 403;
                    throw new Exception("You are not authorized to update this user");
                }

                await _postRepository.DeletePost(post, post.RowVersion);
                await _unitOfWork.CommitAsync();

                await _daprClient.DeleteStateAsync("statestore", postId.ToString());


                result.PostId = post.Id.ToString();
                result.PostName = post.Title;
                result.AuthorId = authId.ToString();
                result.Status = "Deleted";
                result.StatusCode = 200;

                _logger.LogInformation("Post {PostId} deleted successfully", postId);
                return result;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error deleting post {PostId}", postId);
                result.Status = "Error";
                result.Error = ex.Message;
                return result;
            }
        }

    }

}
