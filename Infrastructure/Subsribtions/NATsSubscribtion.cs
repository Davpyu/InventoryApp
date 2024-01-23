
namespace DotNetService.Infrastructure.Subscribtions
{
    public interface ISubscribtionActionAsync<T> {
        Task HandleAsync(T data);
    }

    public interface IReplyAsyncAction<T, R> {
        Task<R> ReplyAsync(T data);
    }
    
    public interface ISubscribtionAction<T> {
        void Handle(T data);
    }

    public interface IReplyAction<T, R> {
        R Reply(T data);
    }
}