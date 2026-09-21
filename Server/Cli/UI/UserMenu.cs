using Entities;
using RepositoryContracts;

namespace Cli.UI;

public class UserMenu
{
    private readonly IUserRepository userRepository;

    public UserMenu(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        bool keepRunning = true;
        while (keepRunning)
        {
            Console.WriteLine();
            Console.WriteLine("--- Manage Users ---");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. Update user");
            Console.WriteLine("3. Delete user");
            Console.WriteLine("4. List all users");
            Console.WriteLine("5. Search users by username");
            Console.WriteLine("0. Back");

            int choice = ConsoleInput.ReadInt("Choose an option: ");
            switch (choice)
            {
                case 1:
                    await CreateUserAsync();
                    break;
                case 2:
                    await UpdateUserAsync();
                    break;
                case 3:
                    await DeleteUserAsync();
                    break;
                case 4:
                    ListUsers();
                    break;
                case 5:
                    SearchUsers();
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

    private async Task CreateUserAsync()
    {
        string userName = ConsoleInput.ReadNonEmptyString("Username: ");

        if (userRepository.GetMany().Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"The username '{userName}' is already taken.");
            return;
        }

        string password = ConsoleInput.ReadNonEmptyString("Password: ");

        User user = new User { UserName = userName, Password = password };
        User created = await userRepository.AddAsync(user);
        Console.WriteLine($"User created with ID {created.Id}.");
    }

    private async Task UpdateUserAsync()
    {
        int id = ConsoleInput.ReadInt("ID of user to update: ");

        User existingUser;
        try
        {
            existingUser = await userRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No user found with ID {id}.");
            return;
        }

        Console.WriteLine($"Leave a field empty to keep the current value ('{existingUser.UserName}').");
        string userName = ConsoleInput.ReadOptionalString($"New username [{existingUser.UserName}]: ");
        string password = ConsoleInput.ReadOptionalString("New password [unchanged]: ");

        if (!string.IsNullOrWhiteSpace(userName))
        {
            existingUser.UserName = userName;
        }

        if (!string.IsNullOrWhiteSpace(password))
        {
            existingUser.Password = password;
        }

        await userRepository.UpdateAsync(existingUser);
        Console.WriteLine("User updated.");
    }

    private async Task DeleteUserAsync()
    {
        int id = ConsoleInput.ReadInt("ID of user to delete: ");

        try
        {
            await userRepository.DeleteAsync(id);
            Console.WriteLine("User deleted.");
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No user found with ID {id}.");
        }
    }

    private void ListUsers()
    {
        List<User> users = userRepository.GetMany().ToList();
        if (!users.Any())
        {
            Console.WriteLine("There are no users yet.");
            return;
        }

        Console.WriteLine();
        foreach (User user in users)
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }

    private void SearchUsers()
    {
        string search = ConsoleInput.ReadNonEmptyString("Search for username containing: ");
        List<User> matches = userRepository.GetMany()
            .Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matches.Any())
        {
            Console.WriteLine("No users matched that search.");
            return;
        }

        Console.WriteLine();
        foreach (User user in matches)
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }
}
