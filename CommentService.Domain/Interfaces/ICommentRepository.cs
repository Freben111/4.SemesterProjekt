namespace CommentService.Domain.Interfaces
{
    public interface ICommentRepository
    {

        Task CreateComment(Comment comment);
        Task UpdateComment(Comment comment, uint rowVersion);
        Task DeleteComment(Comment comment, uint rowVersion);
        Task<Comment> GetCommentById(Guid id);

    }
}
