using FSH.WebApi.Application.Catalog.Commands;
using FSH.WebApi.Application.Catalog.Products;

namespace FSH.WebApi.Host.Controllers.Catalog;

public class CommandsController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Commands)]
    [OpenApiOperation("Search commands using available filters.", "")]
    public Task<PaginationResponse<CommandDto>> SearchAsync(SearchCommandsRequest request)
    {
        return Mediator.Send(request);
    }

}