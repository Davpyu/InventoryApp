using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.UserRole.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Http.API.Version1.UserRole
{
    [Route("api/v1/user-roles")]
    [ApiController]
    public class UserRoleController(
        UserRoleService userRoleService
        ) : ControllerBase
    {
        private readonly UserRoleService _userRoleService = userRoleService;

        // GET: api/UserRole
        [HttpGet()]
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

            return (new ApiResponsePagination(HttpStatusCode.OK, paginate));
        }

        // GET: api/UserRole/5
        [HttpGet("{id}")]
        public async Task<ApiResponse> Show(Guid id)
        {
            var userRoleRepository = await _userRoleService.DetailById(id);
            return (new ApiResponseData(HttpStatusCode.OK, (new UserRoleDetail(userRoleRepository))));
        }

        // POST: api/UserRole
        [HttpPost()]
        [Consumes("application/json")]
        public async Task Store(UserRoleCreateRequest userRoleCreate)
        {
            await _userRoleService.Create(userRoleCreate);
        }

        // PUT: api/UserRole/5
        [HttpPut("{id}")]
        public async Task Update(Guid id, UserRoleUpdateRequest userRoleUpdate)
        {
            await _userRoleService.Update(id, userRoleUpdate);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            await _userRoleService.Delete(id);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }
    }
}
