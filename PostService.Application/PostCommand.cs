using Dapr.Client;
using PostService.Application.Interfaces;
using PostService.Domain;
using PostService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;
using Shared.Post;
using Shared.Post.DTO_s;

namespace PostService.Application
{
    public class PostCommand : IPostCommand
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<PostCommand> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PostCommand(DaprClient daprClient, ILogger<PostCommand> logger, IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _daprClient = daprClient;
            _logger = logger;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        async Task<PostResultMessage> IPostCommand.CreatePost(CreatePostDTO dto)
        {
            var result = new PostResultMessage
            {
                PostName = dto.Title,
                Status = "Creating"
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingPost = _daprClient.GetStateAsync<PostResultMessage>("statestore", dto.Title).Result;
                if (existingPost != null)
                {
                    throw new Exception("Post already exists");
                }

                var post = Post.CreatePost(dto.Title, dto.Content, dto.ForumId);

                await _postRepository.CreatePost(post);
                await _unitOfWork.CommitAsync();

                await _daprClient.SaveStateAsync("statestore", post.Id.ToString(), post);

                _logger.LogInformation("Post {PostName} created successfully", dto.Title);
                result.PostId = post.Id.ToString();
                result.Status = "Created";
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


        async Task<PostResultMessage> IPostCommand.UpdatePost(Guid postId, UpdatePostDTO dto)
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
                    throw new Exception("Post not found");
                }
                post.UpdatePost(dto.Title, dto.Content);
                await _postRepository.UpdatePost(post, post.RowVersion);
                await _unitOfWork.CommitAsync();

                await _daprClient.SaveStateAsync("statestore", post.Title, post);

                result.Status = "Updated";
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

        async Task<PostResultMessage> IPostCommand.DeletePost(Guid postId)
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
                    throw new Exception("Post not found");
                }

                await _postRepository.DeletePost(post, post.RowVersion);
                await _unitOfWork.CommitAsync();

                var cachedPost = await _daprClient.GetStateAsync<Post>("statestore", postId.ToString());
                if (cachedPost != null)
                {
                    await _daprClient.DeleteStateAsync("statestore", postId.ToString());
                }

                _logger.LogInformation("Post {PostId} deleted successfully", postId);
                result.Status = "Deleted";
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
