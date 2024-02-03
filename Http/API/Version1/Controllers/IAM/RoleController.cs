using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.Requests;
using DotNetService.Http.API.Version1.Responses;
using DotNetService.Http.API.Version1.Responses.Role;
using DotNetService.Domain.Role.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Http.API.Version1.Controllers.IAM
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        // GET: api/Role
        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            if (query.Pagination)
            {
                var rolesRepo = _roleService.GetList(query.Search, query.Page, query.PerPage);
                int count = _roleService.Count(query.Search);
                decimal pageInCount = ((decimal)count) / query.PerPage;
                PaginationModel paginate = new PaginationModel()
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Data = RoleItem.MapRepo(rolesRepo),
                    Total = count
                };

                return new ApiResponsePagination(HttpStatusCode.OK, paginate);
            }
            else
            {
                var rolesRepo = _roleService.GetList(query.Search, query.Page, query.PerPage);
                return new ApiResponseDataList(HttpStatusCode.OK, rolesRepo, rolesRepo.Count);
            }
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            var role = _roleService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new RoleDetail(role));
        }
    }
}
