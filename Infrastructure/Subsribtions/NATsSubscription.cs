
namespace DotNetService.Infrastructure.Subscriptions
{
    public interface ISubscriptionActionAsync<T> {
        Task HandleAsync(T data);
    }

    public interface IReplyAsyncAction<T, R> {
        Task<R> ReplyAsync(T data);
    }
    
    public interface ISubscriptionAction<T> {
        void Handle(T data);
    }

    public interface IReplyAction<T, R> {
        R Reply(T data);
    }
}