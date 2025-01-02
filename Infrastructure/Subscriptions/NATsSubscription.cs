
namespace DotNetService.Infrastructure.Subscriptions
{
    public interface ISubscriptionActionAsync<in T>
    {
        Task HandleAsync(T data);
    }

    public interface IReplyAsyncAction<TDat, TRes>
    {
        Task<TRes> ReplyAsync(TDat data);
    }

    public interface ISubscriptionAction<in T>
    {
        void Handle(T data);
    }

    public interface IReplyAction<in T, out R>
    {
        R ReplyAsync(T data);
    }
}