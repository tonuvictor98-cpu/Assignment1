using FileRepositories;
using RepositoryContracts;
using Cli.UI;

IUserRepository userRepository = new UserFileRepository();
IPostRepository postRepository = new PostFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();

CliApp app = new CliApp(userRepository, postRepository, commentRepository);
await app.RunAsync();
