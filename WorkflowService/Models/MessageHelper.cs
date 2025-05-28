using Dapr.Workflow;
using Shared.Comment;
using Shared.Forum;
using Shared.Post;

namespace WorkflowService.Models
{
    public class MessageHelper
    {

        public static ForumMessage ConvertToForum(DeleteForumWorkflowModel model, string workflowid)
        {
            return new ForumMessage()
            {
                ForumId = model.ForumId.ToString(),
                JWT = model.JWT,
                WorkflowId = workflowid,

            };
        }

        public static PostMessage ConvertToPost(DeleteForumWorkflowModel model, string workflowid)
        {
            return new PostMessage()
            {
                ForumId = model.ForumId,
                JWT = model.JWT,
                WorkflowId = workflowid,
            };
        }

        public static CommentMessage ConvertToComment(DeleteForumWorkflowModel model, string workflowid)
        {
            return new CommentMessage()
            {
                PostIds = model.PostIds,
                JWT = model.JWT,
                WorkflowId = workflowid
            };

        }
    }
}

            //public static T FillMessage<T>(WorkflowActivityContext context, Order order) where T : WorkflowMessage, new()
            //{
            //    var result = new T();

            //    var customer = new CustomerDto
            //    {
            //        Address = order.Customer.Address,
            //        Name = order.Customer.Name,
            //        Phone = order.Customer.Phone
            //    };

            //    var examType = typeof(T);
            //    // Change the instance property value.
            //    var workflowId = examType.GetProperty("WorkflowId");
            //    workflowId.SetValue(result, context.InstanceId);

            //    // Change the instance property value.
            //    var orderId = examType.GetProperty("OrderId");
            //    orderId.SetValue(result, order.OrderId);

            //    var pizzaType = examType.GetProperty("PizzaType");
            //    pizzaType.SetValue(result, order.PizzaType);

            //    var size = examType.GetProperty("Size");
            //    size.SetValue(result, order.Size);

            //    var customerProp = examType.GetProperty("Customer");
            //    customerProp.SetValue(result, customer);

            //    return result;
            //}