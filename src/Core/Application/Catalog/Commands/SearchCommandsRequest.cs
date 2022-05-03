using System;
using FSH.WebApi.Domain.Command;

namespace FSH.WebApi.Application.Catalog.Commands
{
    public class SearchCommandsRequest : PaginationFilter, IRequest<PaginationResponse<CommandDto>>
    {
        public Guid? EntityId { get; set; }
    }

    public class CommandsBySearchSpec : EntitiesByPaginationFilterSpec<Command, CommandDto>
    {
        public CommandsBySearchSpec(SearchCommandsRequest request)
            : base(request) =>
            Query.OrderBy(c => c.StartTime, !request.HasOrderBy())
            .Where(p => p.EntityId.Equals(request.EntityId!.Value), request.EntityId.HasValue);
    }

    public class SearchCommandsRequestHandler : IRequestHandler<SearchCommandsRequest, PaginationResponse<CommandDto>>
    {
        private readonly IReadRepository<Command> _repository;

        public SearchCommandsRequestHandler(IReadRepository<Command> repository) => _repository = repository;

        public async Task<PaginationResponse<CommandDto>> Handle(SearchCommandsRequest request, CancellationToken cancellationToken)
        {
            var spec = new CommandsBySearchSpec(request);
            return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}

