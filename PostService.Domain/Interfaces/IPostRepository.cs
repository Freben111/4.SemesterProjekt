namespace PostService.Domain.Interfaces
{
    public interface IPostRepository
    {

        Task CreatePost(Post post);
        Task UpdatePost(Post post, uint rowVersion);
        Task DeletePost(Post post, uint rowVersion);
        Task<Post> GetPostById(Guid id);

    }
}
