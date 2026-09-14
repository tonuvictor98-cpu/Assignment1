using Entities;
using RepositoryContracts;

namespace Cli.UI;

public class PostMenu
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public PostMenu(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    public async Task RunAsync()
    {
        bool keepRunning = true;
        while (keepRunning)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Posts ---");
            Console.WriteLine("1. Create post");
            Console.WriteLine("2. Update post");
            Console.WriteLine("3. Delete post");
            Console.WriteLine("4. View posts overview");
            Console.WriteLine("5. View single post");
            Console.WriteLine("6. Filter posts by user ID");
            Console.WriteLine("7. Search posts by title");
            Console.WriteLine("0. Back");

            int choice = ConsoleInput.ReadInt("Choose an option: ");
            switch (choice)
            {
                case 1:
                    await CreatePostAsync();
                    break;
                case 2:
                    await UpdatePostAsync();
                    break;
                case 3:
                    await DeletePostAsync();
                    break;
                case 4:
                    ViewOverview();
                    break;
                case 5:
                    await ViewSinglePostAsync();
                    break;
                case 6:
                    await FilterByUserAsync();
                    break;
                case 7:
                    SearchByTitle();
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

    private async Task CreatePostAsync()
    {
        int userId = ConsoleInput.ReadInt("Your user ID: ");
        if (!await UserExistsAsync(userId))
        {
            Console.WriteLine($"No user found with ID {userId}.");
            return;
        }

        string title = ConsoleInput.ReadNonEmptyString("Title: ");
        string body = ConsoleInput.ReadNonEmptyString("Body: ");

        Post post = new Post { Title = title, Body = body, UserId = userId };
        Post created = await postRepository.AddAsync(post);
        Console.WriteLine($"Post created with ID {created.Id}.");
    }

    private async Task UpdatePostAsync()
    {
        int id = ConsoleInput.ReadInt("ID of post to update: ");

        Post existingPost;
        try
        {
            existingPost = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with ID {id}.");
            return;
        }

        Console.WriteLine("Leave a field empty to keep the current value.");
        string title = ConsoleInput.ReadOptionalString($"New title [{existingPost.Title}]: ");
        string body = ConsoleInput.ReadOptionalString("New body [unchanged]: ");

        if (!string.IsNullOrWhiteSpace(title))
        {
            existingPost.Title = title;
        }

        if (!string.IsNullOrWhiteSpace(body))
        {
            existingPost.Body = body;
        }

        await postRepository.UpdateAsync(existingPost);
        Console.WriteLine("Post updated.");
    }

    private async Task DeletePostAsync()
    {
        int id = ConsoleInput.ReadInt("ID of post to delete: ");

        try
        {
            await postRepository.DeleteAsync(id);
            Console.WriteLine("Post deleted.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with ID {id}.");
        }
    }

    private void ViewOverview()
    {
        List<Post> posts = postRepository.GetMany().ToList();
        if (!posts.Any())
        {
            Console.WriteLine("There are no posts yet.");
            return;
        }

        Console.WriteLine();
        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }

    private async Task ViewSinglePostAsync()
    {
        int id = ConsoleInput.ReadInt("Post ID: ");

        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with ID {id}.");
            return;
        }

        string authorName = await GetUserNameAsync(post.UserId);

        Console.WriteLine();
        Console.WriteLine($"[{post.Id}] {post.Title}");
        Console.WriteLine($"By: {authorName}");
        Console.WriteLine(post.Body);
        Console.WriteLine();
        Console.WriteLine("Comments:");

        List<Comment> comments = commentRepository.GetMany().Where(c => c.PostId == id).ToList();
        if (!comments.Any())
        {
            Console.WriteLine("No comments yet.");
            return;
        }

        foreach (Comment comment in comments)
        {
            string commenterName = await GetUserNameAsync(comment.UserId);
            Console.WriteLine($"- {commenterName}: {comment.Body}");
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

        List<Post> posts = postRepository.GetMany().Where(p => p.UserId == userId).ToList();
        if (!posts.Any())
        {
            Console.WriteLine("This user has no posts.");
            return;
        }

        Console.WriteLine();
        foreach (Post post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }

    private void SearchByTitle()
    {
        string search = ConsoleInput.ReadNonEmptyString("Search for title containing: ");
        List<Post> matches = postRepository.GetMany()
            .Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matches.Any())
        {
            Console.WriteLine("No posts matched that search.");
            return;
        }

        Console.WriteLine();
        foreach (Post post in matches)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
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

    private async Task<string> GetUserNameAsync(int userId)
    {
        try
        {
            User user = await userRepository.GetSingleAsync(userId);
            return user.UserName;
        }
        catch (InvalidOperationException)
        {
            return "unknown user";
        }
    }
}
