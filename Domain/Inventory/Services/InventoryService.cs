using DotNetService.Domain.Inventory.Dtos;
using DotNetService.Domain.Inventory.Messages;
using DotNetService.Domain.Inventory.Repositories;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Infrastructure.Exceptions;

namespace DotNetService.Domain.Inventory.Services
{
    public class InventoryService(
        InventoryStoreRepository inventoryStoreRepository,
        InventoryQueryRepository inventoryQueryRepository
    )
    {
        private readonly InventoryStoreRepository _inventoryStoreRepository = inventoryStoreRepository;
        private readonly InventoryQueryRepository _inventoryQueryRepository = inventoryQueryRepository;

        public async Task<PaginationModel<InventoryResultDto>> Index(InventoryQueryDto query = null)
        {
            var data = await _inventoryQueryRepository.Pagination(query);
            var formatedData = InventoryResultDto.MapRepo(data.Data);
            var paginate = PaginationModel<InventoryResultDto>.Parse(formatedData, data.Count, query);
            return paginate;
        }

        public async Task Create(InventoryCreateDto dataCreate)
        {
            var isInventoryExist = await _inventoryQueryRepository.IsExistByName(dataCreate.Name);

            if (isInventoryExist)
            {
                throw new UnprocessableEntityException(InventoryErrorMessage.ErrInventoryAlreadyExist);
            }

            var data = InventoryCreateDto.Assign(dataCreate);

            await _inventoryStoreRepository.Create(data);
        }

        public async Task<InventoryResultDto> Detail(Guid id)
        {
            var inventory = await _inventoryQueryRepository.FindOneById(id);
            if (inventory == null)
            {
                throw new DataNotFoundException(InventoryErrorMessage.ErrInventoryNotFound);
            }

            return new InventoryResultDto(inventory);
        }

        public async Task Update(Guid id, InventoryUpdateDto dataUpdate)
        {
            var data = InventoryUpdateDto.Assign(dataUpdate);
            await _inventoryStoreRepository.Update(id, data);
        }

        public async Task Delete(Guid id)
        {
            await _inventoryStoreRepository.Delete(id);
        }
    }
}