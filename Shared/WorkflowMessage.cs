namespace Shared
{
    public abstract class WorkflowMessage
    {
        public string WorkflowId { get; set; }
    }

    public class UserDto
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
    }
}
