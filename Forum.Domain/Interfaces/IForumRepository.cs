namespace ForumService.Domain.Interfaces
{
    public interface IForumRepository
    {

        Task CreateForum(Forum forum);
        Task UpdateForum(Forum forum, uint rowVersion);
        Task DeleteForum(Forum forum, uint rowVersion);
        Task<Forum> GetForumById(Guid id);

    }
}
