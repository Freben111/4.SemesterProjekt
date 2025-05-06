namespace UserService.Domain.Interfaces
{
    public interface IUserRepository
    {

        Task CreateUser(User user);
        Task UpdateUser(User user, uint rowVersion);
        Task DeleteUser(User user, uint rowVersion);
        Task<User> GetUserById(Guid id);
        Task<User> GetUserByUsername(string username);

    }
}
