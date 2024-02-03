using Microsoft.AspNetCore.Mvc;
using DotNetService.Http.API.Version1.Requests;
using DotNetService.Http.API.Version1.Responses;
using DotNetService.Http.API.Version1.Responses.Permission;
using DotNetService.Domain.Permission.Services;
using System.Net;
using DotNetService.Infrastructure.Shareds;

namespace DotNetService.Http.API.Version1.Controllers.IAM
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionService _permissionService;

        // GET: api/Permission
        [HttpGet()]
        public ApiResponse Index([FromQuery] Query query, [FromHeader] Header header)
        {
            if (query.Pagination)
            {
                var permissionsRepo = _permissionService.GetList(query.Search, query.Page, query.PerPage);
                int count = _permissionService.Count(query.Search);
                decimal pageInCount = ((decimal)count) / query.PerPage;
                var paginate = new PaginationModel()
                {
                    TotalPage = (int)Math.Ceiling(pageInCount),
                    Page = query.Page,
                    PerPage = query.PerPage,
                    Data = PermissionItem.MapRepo(permissionsRepo),
                    Total = count
                };

                return new ApiResponsePagination(HttpStatusCode.OK, paginate);
            }
            else
            {
                var permissionsRepo = _permissionService.GetList(query.Search, query.Page, query.PerPage);
                return new ApiResponseDataList(HttpStatusCode.OK, permissionsRepo, permissionsRepo.Count);
            }
        }

        // GET: api/Permission/5
        [HttpGet("{id}")]
        public ApiResponse Show(Guid id)
        {
            var permissionRepository = _permissionService.DetailById(id);
            return new ApiResponseData(HttpStatusCode.OK, new PermissionDetail(permissionRepository));
        }
    }
}
