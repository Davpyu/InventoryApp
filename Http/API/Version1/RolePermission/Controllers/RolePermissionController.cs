using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.RolePermission;
using System.Net;
using DotNetService.Applications.RolePermission.Service;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;

namespace DotNetService.Http.API.Version1.RolePermission
{
    [Route("api/v1/role-permission")]
    [ApiController]
    public class RolePermissionController(RolePermissionService rolePermissionService)
        : ControllerBase
    {
        private readonly RolePermissionService _rolePermissionService = rolePermissionService;

        // GET: api/RolePermission
        [HttpGet()]
        [Permissions(PermissionConstant.ROLE_VIEW, PermissionConstant.PERMISSION_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] Query query, [FromHeader] Header header)
        {
            var rolePermissionsRepo = await _rolePermissionService.GetList(query.Page, query.PerPage);
            int count = await _rolePermissionService.Count();
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
        [Permissions(PermissionConstant.ROLE_VIEW, PermissionConstant.PERMISSION_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            var rolePermissionRepository = await _rolePermissionService.DetailById(id);
            return (new ApiResponseData(HttpStatusCode.OK, (new RolePermissionDetail(rolePermissionRepository))));
        }

        // POST: api/RolePermission
        [HttpPost()]
        [Consumes("application/json")]
        [Permissions(PermissionConstant.ROLE_CREATE, PermissionConstant.PERMISSION_CREATE)]
        public async Task<ApiResponse> Store(RolePermissionCreateRequest rolePermissionCreate)
        {
            await _rolePermissionService.Create(rolePermissionCreate);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }

        // PUT: api/RolePermission/5
        [HttpPut("{id}")]
        [Permissions(PermissionConstant.ROLE_UPDATE, PermissionConstant.PERMISSION_UPDATE)]
        public async Task<ApiResponse> Update(Guid id, RolePermissionUpdateRequest rolePermissionUpdate)
        {
            await _rolePermissionService.Update(id, rolePermissionUpdate);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.ROLE_DELETE, PermissionConstant.PERMISSION_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _rolePermissionService.Delete(id);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }
    }
}
