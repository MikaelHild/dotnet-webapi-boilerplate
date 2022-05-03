using System;
namespace FSH.WebApi.Application.Common.Commands
{
    public interface ICommand : IRequest
    {
        string? JobId { get; set; }
        int RunId { get; set; }
        int? ParentRunId { get; }
        int SourceId { get; }
        CommandStatus Status { get; set; }
        string StartedByUser { get; }
        string StartedByUserName { get; }
    }

}

