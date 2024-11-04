using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.UserRole.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Constants.Permission;

namespace DotNetService.Http.API.Version1.UserRole
{
    [Route("api/v1/user-roles")]
    [ApiController]
    public class UserRoleController(
        UserRoleService userRoleService
        ) : ControllerBase
    {
        private readonly UserRoleService _userRoleService = userRoleService;

        [HttpGet()]
        [Permissions(PermissionConstant.USER_VIEW, PermissionConstant.ROLE_VIEW)]
        public async Task<ApiResponse> Index([FromQuery] Query query, [FromHeader] Header header)
        {
            var userRolesRepo = await _userRoleService.GetList(query.Page, query.PerPage);
            int count = await _userRoleService.Count(query.Search);
            decimal pageInCount = ((decimal)count) / query.PerPage;
            PaginationModel paginate = (new PaginationModel()
            {
                TotalPage = (int)Math.Ceiling(pageInCount),
                Page = query.Page,
                PerPage = query.PerPage,
                Data = UserRoleItem.MapRepo(userRolesRepo),
                Total = count
            });

            return new ApiResponsePagination(HttpStatusCode.OK, paginate);
        }

        [HttpGet("{id}")]
        [Permissions(PermissionConstant.USER_VIEW, PermissionConstant.ROLE_VIEW)]
        public async Task<ApiResponse> Show(Guid id)
        {
            var userRoleRepository = await _userRoleService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new UserRoleDetail(userRoleRepository));
        }

        [HttpPost()]
        [Consumes("application/json")]
        [Permissions(PermissionConstant.USER_CREATE, PermissionConstant.ROLE_CREATE)]
        public async Task Store(UserRoleCreateRequest userRoleCreate)
        {
            await _userRoleService.Create(userRoleCreate);
        }

        [HttpPut("{id}")]
        [Permissions(PermissionConstant.USER_UPDATE, PermissionConstant.ROLE_UPDATE)]
        public async Task Update(Guid id, UserRoleUpdateRequest userRoleUpdate)
        {
            await _userRoleService.Update(id, userRoleUpdate);
        }

        [HttpDelete("{id}")]
        [Permissions(PermissionConstant.USER_DELETE, PermissionConstant.ROLE_DELETE)]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _userRoleService.Delete(id);
            return new ApiResponseData(HttpStatusCode.OK, null);
        }
    }
}
