using Microsoft.AspNetCore.Mvc;
using System.Net;
using DotNetService.Domain.Inventory.Services;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;
using DotNetService.Domain.Inventory.Dtos;
using DotNetService.Infrastructure.Helpers;

namespace DotNetService.Http.API.Version1.Inventory.Controllers
{
    [Route("api/v1/inventories")]
    [ApiController]

    public class InventoryController(
        InventoryService inventoryService
        ) : ControllerBase
    {
        private readonly InventoryService _inventoryService = inventoryService;

        [HttpGet()]
        [Permissions(PermissionConstant.INVENTORY_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] InventoryQueryDto query)
        {
            var paginationResult = await _inventoryService.Index(query);
            return new ApiResponsePagination<InventoryResultDto>(HttpStatusCode.OK, paginationResult);
        }

        [HttpGet("{id}")]
        [Permissions(PermissionConstant.INVENTORY_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            Models.Inventory data = await _inventoryService.Detail(id);
            return new ApiResponseData<Models.Inventory>(HttpStatusCode.OK, data);
        }

        [HttpPost()]
        [Permissions(PermissionConstant.INVENTORY_CREATE)]
        public async Task<ApiResponse> Store(InventoryCreateDto dataCreate)
        {
            await _inventoryService.Create(dataCreate);
            return new ApiResponseData<Models.Inventory>(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        [Permissions(PermissionConstant.INVENTORY_UPDATE)]
        public async Task<ApiResponse> Update(Guid id, InventoryUpdateDto dataUpdate)
        {
            await _inventoryService.Update(id, dataUpdate);
            return new ApiResponseData<Models.Inventory>(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.INVENTORY_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _inventoryService.Delete(id);
            return new ApiResponseData<Models.Inventory>(HttpStatusCode.OK, null);
        }
    }
}
