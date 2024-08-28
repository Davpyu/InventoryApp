using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.RolePermission;
using System.Net;
using DotNetService.Applications.RolePermission.Service;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Authorization;

namespace DotNetService.Http.API.Version1.RolePermission
{
    [Route("api/v1/role-permission")]
    [ApiController]
    [AllowAnonymous]
    public class RolePermissionController(RolePermissionService rolePermissionService)
        : ControllerBase
    {
        private readonly RolePermissionService _rolePermissionService = rolePermissionService;

        // GET: api/RolePermission
        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            var rolePermissionsRepo = _rolePermissionService.GetList(query.Page, query.PerPage);
            int count = _rolePermissionService.Count();
            decimal pageInCount = ((decimal)count) / query.PerPage;
                PaginationModel paginate = (new PaginationModel()
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Data = RolePermissionItem.MapRepo(rolePermissionsRepo),
                    Total = count
                });

            return (new ApiResponsePagination(HttpStatusCode.OK, paginate));
        }

        // GET: api/RolePermission/5
        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            var rolePermissionRepository = _rolePermissionService.DetailById(id);
            return (new ApiResponseData(HttpStatusCode.OK, (new RolePermissionDetail(rolePermissionRepository))));
        }

        // POST: api/RolePermission
        [HttpPost()]
        [Consumes("application/json")]
        public ApiResponse Store(RolePermissionCreateRequest rolePermissionCreate)
        {
            _rolePermissionService.Create(rolePermissionCreate);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }

        // PUT: api/RolePermission/5
        [HttpPut("{id}")]
        public ApiResponse Update(Guid id, RolePermissionUpdateRequest rolePermissionUpdate)
        {
            _rolePermissionService.Update(id, rolePermissionUpdate);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ApiResponse Delete(Guid id)
        {
            _rolePermissionService.Delete(id);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }
    }
}
