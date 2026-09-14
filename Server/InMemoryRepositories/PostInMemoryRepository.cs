using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new List<Post>();

    public PostInMemoryRepository()
    {
        posts.Add(new Post { Id = 1, Title = "bs 01", Body = "Welcome to assignment 2, a world without rules", UserId = 1 });
        posts.Add(new Post { Id = 2, Title = "bs 02", Body = "You the new guy huh welcome to a2.", UserId = 2 });
        posts.Add(new Post { Id = 3, Title = "bs 03", Body = "welcome to dnp kid, if you have dreams consider them gone", UserId = 1 });
        posts.Add(new Post { Id = 4, Title = "bs 04", Body = "you're in a2 now, my project, my rules, be careful", UserId = 3 });
    }

    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max(x => x.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(x => x.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(x => x.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(x => x.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}
