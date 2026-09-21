using RepositoryContracts;

namespace Cli.UI;

public class CliApp
{
    private readonly UserMenu userMenu;
    private readonly PostMenu postMenu;
    private readonly CommentMenu commentMenu;

    public CliApp(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        userMenu = new UserMenu(userRepository);
        postMenu = new PostMenu(postRepository, userRepository, commentRepository);
        commentMenu = new CommentMenu(commentRepository, postRepository, userRepository);
    }

    public async Task RunAsync()
    {
        bool keepRunning = true;
        while (keepRunning)
        {
            Console.WriteLine();
            Console.WriteLine("=== Forum ===");
            Console.WriteLine("1. Manage users");
            Console.WriteLine("2. Manage posts");
            Console.WriteLine("3. Manage comments");
            Console.WriteLine("0. Exit");

            int choice = ConsoleInput.ReadInt("Choose an option: ");
            switch (choice)
            {
                case 1:
                    await userMenu.RunAsync();
                    break;
                case 2:
                    await postMenu.RunAsync();
                    break;
                case 3:
                    await commentMenu.RunAsync();
                    break;
                case 0:
                    keepRunning = false;
                    break;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }

        Console.WriteLine("Goodbye!");
    }
}
