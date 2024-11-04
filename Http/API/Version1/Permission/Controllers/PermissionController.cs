using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.Permission.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;

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
        [Permissions(PermissionConstant.PERMISSION_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] PermissionQueryRequest query)
        {
            return await _permissionService.Index(query);
        }

        [HttpGet("{id}")]
        [Permissions(PermissionConstant.PERMISSION_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            var permissionRepository = await _permissionService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new PermissionResponse(permissionRepository));
        }

        [HttpPost()]
        [Permissions(PermissionConstant.PERMISSION_CREATE)]
        public async Task<ApiResponse> Store(PermissionCreateRequest dataCreate)
        {
            await _permissionService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpPut("{id}")]
        [Permissions(PermissionConstant.PERMISSION_UPDATE)]
        public async Task<ApiResponse> Update(Guid id, PermissionUpdateRequest dataUpdate)
        {
            await _permissionService.Update(id, dataUpdate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.PERMISSION_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _permissionService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
