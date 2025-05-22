namespace WorkflowService.Models
{
    public class DeleteForumWorkflowModel
    {
        public Guid ForumId { get; set; }
        public Guid UserId { get; set; }
        public string? JWT { get; set; }
        public string Status { get; set; }
        public string? Error { get; set; }
        public List<Guid> PostIds { get; set; } = new List<Guid>();
    }
}
