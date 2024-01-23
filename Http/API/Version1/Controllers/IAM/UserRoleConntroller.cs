using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.Requests;
using DotNetService.Http.API.Version1.Requests.UserRole;
using DotNetService.Http.API.Version1.Responses;
using DotNetService.Http.API.Version1.Responses.UserRole;
using DotNetService.Domain.UserRole.Services;
using System.Net;

namespace DotNetService.Http.API.Version1.Controllers.IAM
{
    [Route("api/user-role")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;

        public UserRoleController(
            UserRoleService userRoleService
        )
        {
            _userRoleService = userRoleService;
        }

        // GET: api/UserRole
        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            if (query.Pagination)
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
            else
            {
                var userRolesRepo = _userRoleService.GetList(query.Page, query.PerPage);
                return (new ApiResponseDataList(HttpStatusCode.OK, userRolesRepo, userRolesRepo.Count));
            }
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
        public void Store(UserRoleCreate userRoleCreate)
        {
            _userRoleService.Create(userRoleCreate);
        }

        // PUT: api/UserRole/5
        [HttpPut("{id}")]
        public void Update(Guid id, UserRoleUpdate userRoleUpdate)
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
