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

        // GET: api/Permission
        [HttpGet()]
        public ApiResponse Index([FromQuery] PermissionQueryRequest query)
        {
            return _permissionService.Index(query);
        }

        // GET: api/Permission/5
        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            var permissionRepository = _permissionService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new PermissionResponse(permissionRepository));
        }

        [HttpPost()]
        public ApiResponse Store(PermissionCreateRequest dataCreate)
        {
            var data = _permissionService.Create(dataCreate);
            return new ApiResponseData(HttpStatusCode.OK, new PermissionResponse(data));
        }
    }
}
