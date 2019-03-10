using WebApiPaginatedCrud.Models;

namespace WebApiPaginatedCrud.Dtos.Responses.Shared
{
    public abstract class PagedDto : SuccessResponse
    {
        public PageMeta PageMeta { get; set; }
    }
}