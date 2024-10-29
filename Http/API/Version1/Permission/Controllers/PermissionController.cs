using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.Permission.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Http.API.Version1.Permission
{
    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionController(
        PermissionService permissionService
        ) : ControllerBase
    {
        private readonly PermissionService _permissionService = permissionService;

        [HttpGet()]
        public async Task<ApiResponse> Index([FromQuery] PermissionQueryRequest query)
        {
            return await _permissionService.Index(query);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> Show(Guid id)
        {
            var permissionRepository = await _permissionService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new PermissionResponse(permissionRepository));
        }

        [HttpPost()]
        public async Task<ApiResponse> Store(PermissionCreateRequest dataCreate)
        {
            await _permissionService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Update(Guid id, PermissionUpdateRequest dataUpdate)
        {
            await _permissionService.Update(id, dataUpdate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _permissionService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
