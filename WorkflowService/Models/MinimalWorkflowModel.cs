namespace WorkflowService.Models
{
    public class MinimalWorkflowInput
    {
        public string Message { get; set; } = "Hello from minimal workflow!";
        public Guid WorkFlowId { get; set; }
    }
}
