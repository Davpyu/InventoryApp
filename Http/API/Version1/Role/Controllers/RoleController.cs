using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.Role.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace DotNetService.Http.API.Version1.Role
{
    [Route("api/v1/roles")]
    [ApiController]
    [AllowAnonymous]
    public class RoleController(
        RoleService roleService
        ) : ControllerBase
    {
        private readonly RoleService _roleService = roleService;

        // GET: api/Role
        [HttpGet()]
        public ApiResponse Index([FromQuery] RoleQueryRequest query)
        {
            return _roleService.Index(query);
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            var role = _roleService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new RoleResponse(role));
        }

        [HttpPost()]
        public ApiResponse Store(RoleCreateRequest dataCreate)
        {
            var data = _roleService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, new RoleResponse(data));
        }

        [HttpPut("{id}")]
        public ApiResponse Update(Guid id, RoleUpdateRequest dataUpdate)
        {
            var data = _roleService.Update(id, dataUpdate);
            return new ApiResponseData(HttpStatusCode.OK, new RoleResponse(data));
        }

        [HttpDelete("{id}")]
        public ApiResponse Delete(Guid id)
        {
            _roleService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
