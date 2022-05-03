using System;
namespace FSH.WebApi.Application.Catalog.Commands
{
    public class CommandDto : IDto
    {
        public string? CommandType { get; private set; } = default!;
        public string? CommandJson { get; private set; } = default!;
        public string? JobId { get; private set; } = default!;
        public int? SourceId { get; private set; }
        public int? ParentRunId { get; private set; }
        public string? StartedByUserId { get; private set; } = default!;
        public string? StartedByUserName { get; private set; } = default!;
        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset? EndTime { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? Exception { get; private set; }
        public bool? Dismissed { get; private set; }
    }
}

