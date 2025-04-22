using Dapr.Client;
using Forum.Domain;
using ForumService.Application.Interfaces;
using ForumService.Application.ServiceDTO;
using Microsoft.Extensions.Logging;
using Shared.Forum;

namespace ForumService.Application
{
    public class ForumApplication : IForumApplication
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<ForumApplication> _logger;
        public ForumApplication(DaprClient daprClient, ILogger<ForumApplication> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        Task<ForumResultMessage> IForumApplication.CreateForum(CreateForumDTO dto)
        {


            var result = new ForumResultMessage
            {
                ForumName = dto.Name,
                Status = "Creating"
            };


            try
            {
                _logger.LogInformation("Creating forum with name: {ForumName}", dto.Name);

                // Check if the forum already exists
                var existingForum = _daprClient.GetStateAsync<ForumResultMessage>("statestore", dto.Name).Result;
                if (existingForum != null)
                {
                    throw new Exception("Forum already exists");

                }

                var forum = Forum.Domain.Forum.CreateForum(dto.Name, dto.Description);

                result.ForumId = forum.Id.ToString();
                result.Status = "Created";


                return Task.FromResult(result);



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating forum {ForumName}", dto.Name);
                result.Status = "Error";
                result.Error = ex.Message;
                return Task.FromResult(result);
            }


        }
    }

}
