using DotNetService.Constants.Logger;
using DotNetService.Domain.Inventory.Repositories;
using DotNetService.Infrastructure.Subscriptions;

namespace DotNetService.Domain.Inventory.Listeners
{
    public class CheckInventoryListenAndReply(
        ILoggerFactory loggerFactory,
        InventoryQueryRepository inventoryQueryRepository
    ) : IReplyAsyncAction<IDictionary<string, object>, IDictionary<string, object>>
    {
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.ACTIVITY);
        private readonly InventoryQueryRepository _inventoryQueryRepository = inventoryQueryRepository;

        public async Task<IDictionary<string, object>> ReplyAsync(IDictionary<string, object> data)
        {
            // EXAMPLE: Do operation for reply event
            var inventoryId = data.Where(x => x.Key == "inventory_id").First().Value;
            var quantity = data.Where(x => x.Key == "quantity").First().Value;

            var inventory = await _inventoryQueryRepository.FindOneById(Guid.Parse(inventoryId.ToString()));

            var reply = new Dictionary<string, object>();

            if (inventory.Quantity >= Convert.ToInt32(quantity))
            {
                reply.Add("approval", "Approved");
            }

            else
            {
                reply.Add("approval", "Rejected");
            }

            return reply;
        }
    }
}
