using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.Role.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using Newtonsoft.Json;

namespace DotNetService.Http.API.Version1.Role
{
    [Route("api/v1/roles")]
    [ApiController]
    public class RoleController(
        RoleService roleService
        ) : ControllerBase
    {
        private readonly RoleService _roleService = roleService;

        [HttpGet()]
        public async Task<ApiResponse> Index([FromQuery] RoleQueryRequest query)
        {
            return await _roleService.Index(query);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> Show(Guid id)
        {
            var role = await _roleService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new RoleResponse(role));
        }

        [HttpPost()]
        public async Task<ApiResponse> Store(RoleCreateRequest dataCreate)
        {
            var data = await _roleService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, new RoleResponse(data));
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Update(Guid id, RoleUpdateRequest dataUpdate)
        {
            await _roleService.Update(id, dataUpdate);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _roleService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
