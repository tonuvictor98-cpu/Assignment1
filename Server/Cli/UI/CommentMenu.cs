using Entities;
using RepositoryContracts;

namespace Cli.UI;

public class CommentMenu
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CommentMenu(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        bool keepRunning = true;
        while (keepRunning)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Comments ---");
            Console.WriteLine("1. Add comment to post");
            Console.WriteLine("2. Update comment");
            Console.WriteLine("3. Delete comment");
            Console.WriteLine("4. List all comments");
            Console.WriteLine("5. Filter comments by user ID");
            Console.WriteLine("6. Filter comments by post ID");
            Console.WriteLine("0. Back");

            int choice = ConsoleInput.ReadInt("Choose an option: ");
            switch (choice)
            {
                case 1:
                    await AddCommentAsync();
                    break;
                case 2:
                    await UpdateCommentAsync();
                    break;
                case 3:
                    await DeleteCommentAsync();
                    break;
                case 4:
                    ListComments();
                    break;
                case 5:
                    await FilterByUserAsync();
                    break;
                case 6:
                    await FilterByPostAsync();
                    break;
                case 0:
                    keepRunning = false;
                    break;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
    }

    private async Task AddCommentAsync()
    {
        int postId = ConsoleInput.ReadInt("Post ID: ");
        if (!await PostExistsAsync(postId))
        {
            Console.WriteLine($"No post found with ID {postId}.");
            return;
        }

        int userId = ConsoleInput.ReadInt("Your user ID: ");
        if (!await UserExistsAsync(userId))
        {
            Console.WriteLine($"No user found with ID {userId}.");
            return;
        }

        string body = ConsoleInput.ReadNonEmptyString("Comment: ");

        Comment comment = new Comment { Body = body, PostId = postId, UserId = userId };
        Comment created = await commentRepository.AddAsync(comment);
        Console.WriteLine($"Comment added with ID {created.Id}.");
    }

    private async Task UpdateCommentAsync()
    {
        int id = ConsoleInput.ReadInt("ID of comment to update: ");

        Comment existingComment;
        try
        {
            existingComment = await commentRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No comment found with ID {id}.");
            return;
        }

        string body = ConsoleInput.ReadNonEmptyString("New comment text: ");
        existingComment.Body = body;

        await commentRepository.UpdateAsync(existingComment);
        Console.WriteLine("Comment updated.");
    }

    private async Task DeleteCommentAsync()
    {
        int id = ConsoleInput.ReadInt("ID of comment to delete: ");

        try
        {
            await commentRepository.DeleteAsync(id);
            Console.WriteLine("Comment deleted.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No comment found with ID {id}.");
        }
    }

    private void ListComments()
    {
        List<Comment> comments = commentRepository.GetMany().ToList();
        if (!comments.Any())
        {
            Console.WriteLine("There are no comments yet.");
            return;
        }

        Console.WriteLine();
        foreach (Comment comment in comments)
        {
            Console.WriteLine($"[{comment.Id}] (post {comment.PostId}, user {comment.UserId}): {comment.Body}");
        }
    }

    private async Task FilterByUserAsync()
    {
        int userId = ConsoleInput.ReadInt("User ID: ");
        if (!await UserExistsAsync(userId))
        {
            Console.WriteLine($"No user found with ID {userId}.");
            return;
        }

        List<Comment> comments = commentRepository.GetMany().Where(c => c.UserId == userId).ToList();
        if (!comments.Any())
        {
            Console.WriteLine("This user has no comments.");
            return;
        }

        Console.WriteLine();
        foreach (Comment comment in comments)
        {
            Console.WriteLine($"[{comment.Id}] (post {comment.PostId}): {comment.Body}");
        }
    }

    private async Task FilterByPostAsync()
    {
        int postId = ConsoleInput.ReadInt("Post ID: ");
        if (!await PostExistsAsync(postId))
        {
            Console.WriteLine($"No post found with ID {postId}.");
            return;
        }

        List<Comment> comments = commentRepository.GetMany().Where(c => c.PostId == postId).ToList();
        if (!comments.Any())
        {
            Console.WriteLine("This post has no comments.");
            return;
        }

        Console.WriteLine();
        foreach (Comment comment in comments)
        {
            Console.WriteLine($"[{comment.Id}] (user {comment.UserId}): {comment.Body}");
        }
    }

    private async Task<bool> UserExistsAsync(int userId)
    {
        try
        {
            await userRepository.GetSingleAsync(userId);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private async Task<bool> PostExistsAsync(int postId)
    {
        try
        {
            await postRepository.GetSingleAsync(postId);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
