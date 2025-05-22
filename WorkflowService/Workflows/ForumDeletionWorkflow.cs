using Dapr.Workflow;
using Shared.Comment;
using Shared.Forum;
using Shared.Post;
using WorkflowService.Activities;
using WorkflowService.Models;

namespace WorkflowService.Workflows
{
    public class ForumDeletionWorkflow : Workflow<DeleteForumWorkflowModel, DeleteForumWorkflowModel>
    {

        public override async Task<DeleteForumWorkflowModel> RunAsync(WorkflowContext context, DeleteForumWorkflowModel input)
        {
            var deleteForumResult = new ForumResultMessage();
            var deletePostsResult = new PostResultMessage();
            var deleteCommentsResult = new CommentResultMessage();
            try
            {
                // Step 1: Delete the forum
                await context.CallActivityAsync<DeleteForumWorkflowModel>(
                    nameof(DeleteForumActivity), 
                    input);

                deleteForumResult = await context.WaitForExternalEventAsync<ForumResultMessage>("Forum.Deleted");
                if (deleteForumResult.Status != "Forum Deleted")
                {
                    throw new Exception($"Error deleting forum: {deleteForumResult.Error ?? "Unknown Error"}");
                }
                

                // Step 2: Delete all posts in the forum
                await context.CallActivityAsync<DeleteForumWorkflowModel>(
                    nameof(DeletePostsActivity),
                    input);

                deletePostsResult = await context.WaitForExternalEventAsync<PostResultMessage>("Posts.Deleted");
                if (deletePostsResult.Status != "Posts Deleted")
                {
                    throw new Exception($"Error deleting posts: {deleteForumResult.Error ?? "Unknown Error"}");
                }

                foreach (var post in deletePostsResult.BackupPost)
                {
                    input.PostIds.Add(post.Id);
                }

                // Step 3: Delete all comments in each post
                await context.CallActivityAsync<DeleteForumWorkflowModel>(
                    nameof(DeleteCommentsActivity),
                    input);

                deleteCommentsResult = await context.WaitForExternalEventAsync<CommentResultMessage>("Comments.Deleted");
                if (deleteCommentsResult.Status != "Comments Deleted")
                {
                    throw new Exception($"Error deleting comments: {deleteForumResult.Error ?? "Unknown Error"}");
                }

                input.Status = "Workflow completed succesfully";
                return input;
            }
            catch (Exception ex)
            {
                input.Status = "Error";
                input.Error = ex.Message;

                if (deleteForumResult != null)
                {
                    await context.CallActivityAsync<ForumResultMessage>(
                        nameof(RestoreForumActivity),
                        deleteForumResult);
                }
                if (deletePostsResult != null)
                {
                    await context.CallActivityAsync<PostResultMessage>(
                        nameof(RestorePostsActivity),
                        deletePostsResult);
                }

                return input;
            }
        }
    }
}
