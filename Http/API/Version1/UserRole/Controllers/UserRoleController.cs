using Microsoft.AspNetCore.Mvc;
using DotNetService.Domain.UserRole.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Authorization;

namespace DotNetService.Http.API.Version1.UserRole
{
    [Route("api/v1/user-roles")]
    [ApiController]
     [AllowAnonymous]
    public class UserRoleController(
        UserRoleService userRoleService
        ) : ControllerBase
    {
        private readonly UserRoleService _userRoleService = userRoleService;

        // GET: api/UserRole
        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            var userRolesRepo = _userRoleService.GetList(query.Page, query.PerPage);
            int count = _userRoleService.Count(query.Search);
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
        public ApiResponse Show(Guid id)
        {
            var userRoleRepository = _userRoleService.DetailById(id);
            return (new ApiResponseData(HttpStatusCode.OK, (new UserRoleDetail(userRoleRepository))));
        }

        // POST: api/UserRole
        [HttpPost()]
        [Consumes("application/json")]
        public void Store(UserRoleCreateRequest userRoleCreate)
        {
            _userRoleService.Create(userRoleCreate);
        }

        // PUT: api/UserRole/5
        [HttpPut("{id}")]
        public void Update(Guid id, UserRoleUpdateRequest userRoleUpdate)
        {
            _userRoleService.Update(id, userRoleUpdate);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ApiResponse Delete(Guid id)
        {
            _userRoleService.Delete(id);
            return (new ApiResponseData(HttpStatusCode.OK, null));
        }
    }
}
