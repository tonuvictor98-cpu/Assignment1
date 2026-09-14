namespace Cli.UI;

public static class ConsoleInput
{
    public static string ReadNonEmptyString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    public static string ReadOptionalString(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        return input?.Trim() ?? string.Empty;
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int result))
            {
                return result;
            }

            Console.WriteLine("Please enter a whole number.");
        }
    }
}
